# Helm chart for Catena-X Policy Hub

![Version: 0.1.0-rc1](https://img.shields.io/badge/Version-0.1.0-rc1-informational?style=flat-square) ![Type: application](https://img.shields.io/badge/Type-application-informational?style=flat-square) ![AppVersion: 0.1.0-rc1](https://img.shields.io/badge/AppVersion-0.1.0-rc1-informational?style=flat-square) 

This helm chart installs the Catena-X Policy Hub application v0.1.0-rc1.

For information on how to upgrade from previous versions please refer to [Version Upgrade](./docs/developer/Technical-Documentation/version-upgrade).

For further information please refer to [Technical Documentation](./docs/developer/Technical-Documentation).

The referenced container images are for demonstration purposes only.

## Installation

To install the chart with the release name `policy-hub`:

```shell
$ helm repo add tractusx-dev https://eclipse-tractusx.github.io/charts/dev
$ helm install policy-hub tractusx-dev/policy-hub
```

To install the helm chart into your cluster with your values:

```shell
$ helm install -f your-values.yaml policy-hub tractusx-dev/policy-hub
```

To use the helm chart as a dependency:

```yaml
dependencies:
  - name: policy-hub
    repository: https://eclipse-tractusx.github.io/charts/dev
    version: 0.1.0-rc1
```

## Requirements

| Repository | Name | Version |
|------------|------|---------|
| https://charts.bitnami.com/bitnami | postgresql | 12.12.x |

## Values

| Key | Type | Default | Description |
|-----|------|---------|-------------|
| affinity.podAntiAffinity | object | `{"preferredDuringSchedulingIgnoredDuringExecution":[{"podAffinityTerm":{"labelSelector":{"matchExpressions":[{"key":"app.kubernetes.io/name","operator":"DoesNotExist"}]},"topologyKey":"kubernetes.io/hostname"},"weight":100}]}` | Following Catena-X Helm Best Practices, [reference](https://kubernetes.io/docs/concepts/scheduling-eviction/assign-pod-node/#affinity-and-anti-affinity). |
| centralidpAddress | string | `"https://centralidp.example.org"` | Provide centralidp base address (CX IAM), without trailing '/auth'. |
| dbConnection.schema | string | `"hub"` |  |
| dbConnection.sslMode | string | `"Disable"` |  |
| dotnetEnvironment | string | `"Production"` |  |
| externalDatabase.database | string | `"policy-hub"` | Database name. |
| externalDatabase.existingSecret | string | `"policy-hub-external-db"` | Secret containing the password non-root username, (default 'hub'). |
| externalDatabase.existingSecretPasswordKey | string | `"password"` | Name of an existing secret key containing the database credentials. |
| externalDatabase.host | string | `"policy-hub-postgresql-external-db"` | External PostgreSQL configuration IMPORTANT: non-root db user needs needs to be created beforehand on external database. Database host ('-primary' is added as postfix). |
| externalDatabase.password | string | `""` | Password for the non-root username (default 'hub'). Secret-key 'password'. |
| externalDatabase.port | int | `5432` | Database port number. |
| externalDatabase.user | string | `"hub"` | Non-root username for policy-hub. |
| healthChecks.liveness.path | string | `"/healthz"` |  |
| healthChecks.readyness.path | string | `"/ready"` |  |
| healthChecks.startup.path | string | `"/health/startup"` |  |
| ingress.annotations."nginx.ingress.kubernetes.io/cors-allow-origin" | string | `"https://*.example.org"` | Provide CORS allowed origin. |
| ingress.annotations."nginx.ingress.kubernetes.io/enable-cors" | string | `"true"` |  |
| ingress.annotations."nginx.ingress.kubernetes.io/proxy-body-size" | string | `"8m"` |  |
| ingress.annotations."nginx.ingress.kubernetes.io/use-regex" | string | `"true"` |  |
| ingress.className | string | `"nginx"` |  |
| ingress.enabled | bool | `false` | Policy Hub ingress parameters, enable ingress record generation for policy-hub. |
| ingress.hosts[0] | object | `{"host":"policy-hub.example.org","paths":[{"backend":{"port":8080,"service":"policy-hub-service"},"path":"/api/policy-hub","pathType":"Prefix"}]}` | Provide default path for the ingress record. |
| ingress.name | string | `"policy-hub"` |  |
| ingress.tls[0] | object | `{"hosts":["policy-hub.example.org"],"secretName":""}` | Provide tls secret. |
| ingress.tls[0].hosts | list | `["policy-hub.example.org"]` | Provide host for tls secret. |
| keycloak.central.authRealm | string | `"CX-Central"` |  |
| keycloak.central.jwtBearerOptions.metadataPath | string | `"/auth/realms/CX-Central/.well-known/openid-configuration"` |  |
| keycloak.central.jwtBearerOptions.refreshInterval | string | `"00:00:30"` |  |
| keycloak.central.jwtBearerOptions.requireHttpsMetadata | string | `"true"` |  |
| keycloak.central.jwtBearerOptions.tokenValidationParameters.validAudience | string | `"ClXX-CX-Policy-Hub"` |  |
| keycloak.central.jwtBearerOptions.tokenValidationParameters.validIssuerPath | string | `"/auth/realms/CX-Central"` |  |
| keycloak.central.tokenPath | string | `"/auth/realms/CX-Central/protocol/openid-connect/token"` |  |
| keycloak.central.useAuthTrail | bool | `true` | Flag if the api should be used with an leading /auth path |
| livenessProbe.failureThreshold | int | `3` |  |
| livenessProbe.initialDelaySeconds | int | `10` |  |
| livenessProbe.periodSeconds | int | `10` |  |
| livenessProbe.successThreshold | int | `1` |  |
| livenessProbe.timeoutSeconds | int | `10` |  |
| name | string | `"policy-hub"` |  |
| nodeSelector | object | `{}` | Node labels for pod assignment |
| policyhub.healthChecks.startup.tags[0].name | string | `"HEALTHCHECKS__0__TAGS__1"` |  |
| policyhub.healthChecks.startup.tags[0].value | string | `"policyhubdb"` |  |
| policyhub.image | string | `"tractusx/policy-hub-service:0.1.0-rc1"` |  |
| policyhub.logging.businessLogic | string | `"Information"` |  |
| policyhub.logging.default | string | `"Information"` |  |
| policyhub.name | string | `"policy-hub-service"` |  |
| policyhub.resources | object | `{"requests":{"cpu":"15m","memory":"300M"}}` | We recommend not to specify default resource limits and to leave this as a conscious choice for the user. If you do want to specify resource limits, uncomment the following lines and adjust them as necessary. |
| policyhub.swaggerEnabled | bool | `false` |  |
| policyhubmigrations.image | string | `"tractusx/policy-hub-migrations:0.1.0-rc1"` |  |
| policyhubmigrations.logging.default | string | `"Information"` |  |
| policyhubmigrations.name | string | `"policy-hub-migrations"` |  |
| policyhubmigrations.resources | object | `{"requests":{"cpu":"15m","memory":"105M"}}` | We recommend not to specify default resource limits and to leave this as a conscious choice for the user. If you do want to specify resource limits, uncomment the following lines and adjust them as necessary. |
| policyhubmigrations.seeding.testDataEnvironments | string | `""` |  |
| policyhubmigrations.seeding.testDataPaths | string | `"Seeder/Data"` |  |
| portContainer | int | `8080` |  |
| portService | int | `8080` |  |
| postgresql.architecture | string | `"replication"` |  |
| postgresql.audit.logLinePrefix | string | `"%m %u %d "` |  |
| postgresql.audit.pgAuditLog | string | `"write, ddl"` |  |
| postgresql.auth.database | string | `"policy-hub"` | Database name. |
| postgresql.auth.existingSecret | string | `"policy-hub-postgres"` | Secret containing the passwords for root usernames postgres and non-root username hub. |
| postgresql.auth.username | string | `"hub"` | Non-root username. |
| postgresql.enabled | bool | `true` | PostgreSQL chart configuration; default configurations: host: "policy-hub-postgresql-primary", port: 5432; Switch to enable or disable the PostgreSQL helm chart. |
| postgresql.primary.extendedConfiguration | string | `""` | Extended PostgreSQL Primary configuration (increase of max_connections recommended - default is 100) |
| postgresql.primary.initdb.scriptsConfigMap | string | `"policy-hub-configmap-postgres-init"` |  |
| postgresql.readReplicas.extendedConfiguration | string | `""` | Extended PostgreSQL read only replicas configuration (increase of max_connections recommended - default is 100) |
| readinessProbe.failureThreshold | int | `3` |  |
| readinessProbe.initialDelaySeconds | int | `10` |  |
| readinessProbe.periodSeconds | int | `10` |  |
| readinessProbe.successThreshold | int | `1` |  |
| readinessProbe.timeoutSeconds | int | `1` |  |
| replicaCount | int | `3` |  |
| secrets.postgresql.auth.existingSecret.password | string | `""` | Password for the non-root username 'hub'. Secret-key 'password'. |
| secrets.postgresql.auth.existingSecret.postgrespassword | string | `""` | Password for the root username 'postgres'. Secret-key 'postgres-password'. |
| secrets.postgresql.auth.existingSecret.replicationPassword | string | `""` | Password for the non-root username 'repl_user'. Secret-key 'replication-password'. |
| startupProbe | object | `{"failureThreshold":30,"initialDelaySeconds":10,"periodSeconds":10,"successThreshold":1,"timeoutSeconds":1}` | Following Catena-X Helm Best Practices, [reference](https://github.com/helm/charts/blob/master/stable/nginx-ingress/values.yaml#L210). |
| tolerations | list | `[]` | Tolerations for pod assignment |
| updateStrategy.rollingUpdate.maxSurge | int | `1` |  |
| updateStrategy.rollingUpdate.maxUnavailable | int | `0` |  |
| updateStrategy.type | string | `"RollingUpdate"` | Update strategy type, rolling update configuration parameters, [reference](https://kubernetes.io/docs/concepts/workloads/controllers/statefulset/#update-strategies). |

Autogenerated with [helm docs](https://github.com/norwoodj/helm-docs)
