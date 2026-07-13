# Spec 05 — Segredos do UsersAPI no AWS Secrets Manager.
# O ESO (instalado pela plataforma — spec 01) sincroniza estes secrets para o namespace fcg-<env>
# via ExternalSecret (spec 07/k8s-<env>.yaml). Nenhum valor sensível fica no código/tfstate versionado.
#
# fcg/<env>/common/jwt é criado AQUI (UsersAPI é o emissor dos tokens — precisa da chave privada).
# CatalogAPI e PaymentsAPI só consomem esse secret via ExternalSecret pelo nome (não o recriam).

resource "aws_secretsmanager_secret" "common_jwt" {
  name = "fcg/${var.environment}/common/jwt"
}

resource "aws_secretsmanager_secret_version" "common_jwt" {
  secret_id = aws_secretsmanager_secret.common_jwt.id
  secret_string = jsonencode({
    Jwt__RsaPublicKey  = var.jwt_rsa_public_key_b64
    Jwt__RsaPrivateKey = var.jwt_rsa_private_key_b64
    Jwt__Issuer        = "FiapCloudGames"
    Jwt__Audience      = "FiapCloudGames"
  })
}

resource "aws_secretsmanager_secret" "users_db" {
  name = "fcg/${var.environment}/users/db"
}

resource "aws_secretsmanager_secret_version" "users_db" {
  secret_id = aws_secretsmanager_secret.users_db.id
  secret_string = jsonencode({
    ConnectionStrings__Users = "Host=${split(":", aws_db_instance.users.endpoint)[0]};Port=5432;Database=users_db;Username=postgres;Password=${var.db_password}"
  })
}
