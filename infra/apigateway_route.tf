# Spec 01 (independência) — rotas/integração do UsersAPI no API Gateway central da plataforma.
# Lê api_id/vpc_link_id via SSM (Orchestration/platform) — nunca edita o repo da plataforma.
# O NLB é criado pelo Service k8s (spec 07, type=LoadBalancer interno) — localizado aqui pela
# tag padrão que o EKS/AWS Load Balancer aplicam automaticamente.

data "aws_ssm_parameter" "apigw_api_id" {
  name = "/fcg/platform/apigw/api_id"
}

data "aws_ssm_parameter" "apigw_vpc_link_id" {
  name = "/fcg/platform/apigw/vpc_link_id"
}

data "aws_lb" "users_api" {
  tags = {
    "kubernetes.io/service-name" = "fcg-${var.environment}/users-api"
  }
}

data "aws_lb_listener" "users_api" {
  load_balancer_arn = data.aws_lb.users_api.arn
  port              = 8080
}

resource "aws_apigatewayv2_integration" "users_api" {
  api_id             = data.aws_ssm_parameter.apigw_api_id.value
  integration_type   = "HTTP_PROXY"
  integration_method = "ANY"
  connection_type    = "VPC_LINK"
  connection_id      = data.aws_ssm_parameter.apigw_vpc_link_id.value
  integration_uri    = data.aws_lb_listener.users_api.arn
}

resource "aws_apigatewayv2_route" "users_api" {
  api_id    = data.aws_ssm_parameter.apigw_api_id.value
  route_key = "ANY /api/users/{proxy+}"
  target    = "integrations/${aws_apigatewayv2_integration.users_api.id}"
}

# {proxy+} não casa com o path base sem sufixo (ex.: POST /api/users) — rota exata complementar.
resource "aws_apigatewayv2_route" "users_api_base" {
  api_id    = data.aws_ssm_parameter.apigw_api_id.value
  route_key = "ANY /api/users"
  target    = "integrations/${aws_apigatewayv2_integration.users_api.id}"
}

# JWKS/OpenID discovery (consumido pelas outras APIs para validar JWT).
resource "aws_apigatewayv2_route" "users_api_wellknown" {
  api_id    = data.aws_ssm_parameter.apigw_api_id.value
  route_key = "GET /.well-known/{proxy+}"
  target    = "integrations/${aws_apigatewayv2_integration.users_api.id}"
}
