# Helm chart for Catena-X Policy Hub

![Version: 1.0.0](https://img.shields.io/badge/Version-1.0.0-informational?style=flat-square) ![Type: application](https://img.shields.io/badge/Type-application-informational?style=flat-square) ![AppVersion: 1.0.0](https://img.shields.io/badge/AppVersion-1.0.0-informational?style=flat-square) 

This helm chart installs the Catena-X Policy Hub application v1.0.0.

For information on how to upgrade from previous versions please refer to [Version Upgrade](https://github.com/eclipse-tractusx/policy-hub/tree/v1.0.0/docs/developer/Technical%20Documentation/Version%20Upgrade/portal-upgrade-details.md).

For further information please refer to [Technical Documentation](https://github.com/eclipse-tractusx/policy-hub/tree/v1.0.0/docs/developer/Technical%20Documentation).

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
    version: 1.0.0
```

## Requirements

| Repository | Name | Version |
|------------|------|---------|
| https://charts.bitnami.com/bitnami | postgrespolicyhub(postgresql) | 12.12.x |

## Values

| Key | Type | Default | Description |
|-----|------|---------|-------------|
| affinity.podAntiAffinity | object | `{"preferredDuringSchedulingIgnoredDuringExecution":[{"podAffinityTerm":{"labelSelector":{"matchExpressions":[{"key":"app.kubernetes.io/name","operator":"DoesNotExist"}]},"topologyKey":"kubernetes.io/hostname"},"weight":100}]}` | Following Catena-X Helm Best Practices, [reference](https://kubernetes.io/docs/concepts/scheduling-eviction/assign-pod-node/#affinity-and-anti-affinity). |
| backend.dbConnection.schema | string | `"hub"` |  |
| backend.dbConnection.sslMode | string | `"Disable"` |  |
| backend.dotnetEnvironment | string | `"Production"` |  |
| backend.healthChecks.liveness.path | string | `"/healthz"` |  |
| backend.healthChecks.readyness.path | string | `"/ready"` |  |
| backend.healthChecks.startup.path | string | `"/health/startup"` |  |
| backend.ingress.annotations."nginx.ingress.kubernetes.io/cors-allow-origin" | string | `"https://*.example.org"` | Provide CORS allowed origin. |
| backend.ingress.annotations."nginx.ingress.kubernetes.io/enable-cors" | string | `"true"` |  |
| backend.ingress.annotations."nginx.ingress.kubernetes.io/proxy-body-size" | string | `"8m"` |  |
| backend.ingress.annotations."nginx.ingress.kubernetes.io/use-regex" | string | `"true"` |  |
| backend.ingress.className | string | `"nginx"` |  |
| backend.ingress.enabled | bool | `false` | Policy Hub ingress parameters, enable ingress record generation for policy-hub. |
| backend.ingress.hosts[0] | object | `{"host":"policy-hub.example.org","paths":[{"backend":{"port":8080,"service":"policy-hub-service"},"path":"/api/hub","pathType":"Prefix"}]}` | Provide default path for the ingress record. |
| backend.ingress.name | string | `"policy-hub"` |  |
| backend.ingress.tls[0] | object | `{"hosts":[""],"secretName":""}` | Provide tls secret. |
| backend.ingress.tls[0].hosts | list | `[""]` | Provide host for tls secret. |
| backend.keycloak.central.authRealm | string | `"CX-Central"` |  |
| backend.keycloak.central.jwtBearerOptions.metadataPath | string | `"/auth/realms/CX-Central/.well-known/openid-configuration"` |  |
| backend.keycloak.central.jwtBearerOptions.refreshInterval | string | `"00:00:30"` |  |
| backend.keycloak.central.jwtBearerOptions.requireHttpsMetadata | string | `"true"` |  |
| backend.keycloak.central.jwtBearerOptions.tokenValidationParameters.validAudience | string | `"Cl2-CX-Portal"` |  |
| backend.keycloak.central.jwtBearerOptions.tokenValidationParameters.validIssuerPath | string | `"/auth/realms/CX-Central"` |  |
| backend.keycloak.central.tokenPath | string | `"/auth/realms/CX-Central/protocol/openid-connect/token"` |  |
| backend.keycloak.central.useAuthTrail | bool | `true` | Flag if the api should be used with an leading /auth path |
| backend.policyhub.image.name | string | `"tractusx/portal-hub-service"` |  |
| backend.policyhub.image.policyhubservicetag | string | `"29dbdeb9a8b3e809fdab2406140a226bc55de844"` |  |
| backend.policyhub.keycloakClientId | string | `"Cl2-CX-Portal"` |  |
| backend.policyhub.logging.businessLogic | string | `"Information"` |  |
| backend.policyhub.logging.default | string | `"Information"` |  |
| backend.policyhub.name | string | `"policy-hub-service"` |  |
| backend.policyhub.resources | object | `{}` | We recommend not to specify default resources and to leave this as a conscious choice for the user. If you do want to specify resources, uncomment the following lines, adjust them as necessary, and remove the curly braces after 'resources:'. |
| backend.policyhubmigrations.image.name | string | `"tractusx/policy-hub-migrations"` |  |
| backend.policyhubmigrations.image.policyhubmigrationstag | string | `"29dbdeb9a8b3e809fdab2406140a226bc55de844"` |  |
| backend.policyhubmigrations.logging.default | string | `"Information"` |  |
| backend.policyhubmigrations.name | string | `"policy-hub-migrations"` |  |
| backend.policyhubmigrations.resources | object | `{}` | We recommend not to specify default resources and to leave this as a conscious choice for the user. If you do want to specify resources, uncomment the following lines, adjust them as necessary, and remove the curly braces after 'resources:'. |
| backend.policyhubmigrations.seeding.testDataEnvironments | string | `""` |  |
| backend.policyhubmigrations.seeding.testDataPaths | string | `"Seeder/Data"` | when changing the testDataPath the processIdentity needs to be adjusted as well, or it must be ensured that the identity is existing within the files under the new path |
| centralidpAddress | string | `"https://centralidp.example.org"` | Provide centralidp base address (CX IAM), without trailing '/auth'. |
| externalDatabase.database | string | `"postgres"` | Database name |
| externalDatabase.host | string | `"policy-hub-postgresql-external-db"` | External PostgreSQL configuration IMPORTANT: init scripts (01-init-db-user.sh and 02-init-db.sql) available in templates/configmap-backend-postgres-init.yaml need to be executed beforehand. Database host |
| externalDatabase.policyHubPassword | string | `""` | Password for the non-root username 'hub'. Secret-key 'policy-hub-password'. |
| externalDatabase.policyHubUser | string | `"hub"` | Non-root username for hub. |
| externalDatabase.port | int | `5432` | Database port number |
| externalDatabase.secret | string | `"secret-postgres-external-db"` | Secret containing the passwords non-root usernames portal and provisioning. |
| livenessProbe.failureThreshold | int | `3` |  |
| livenessProbe.initialDelaySeconds | int | `10` |  |
| livenessProbe.periodSeconds | int | `10` |  |
| livenessProbe.successThreshold | int | `1` |  |
| livenessProbe.timeoutSeconds | int | `10` |  |
| name | string | `"policy-hub"` |  |
| nodeSelector | object | `{}` | Node labels for pod assignment |
| portContainer | int | `8080` |  |
| portService | int | `8080` |  |
| postgresql.architecture | string | `"replication"` |  |
| postgresql.audit.logLinePrefix | string | `"%m %u %d "` |  |
| postgresql.audit.pgAuditLog | string | `"write, ddl"` |  |
| postgresql.auth.database | string | `"postgres"` | Database name |
| postgresql.auth.existingSecret | string | `"secret-postgres-init"` | Secret containing the passwords for root usernames postgres and non-root usernames repl_user, portal and provisioning. |
| postgresql.auth.password | string | `""` | Password for the root username 'postgres'. Secret-key 'postgres-password'. |
| postgresql.auth.policyHubPassword | string | `""` | Password for the non-root username 'hub'. Secret-key 'policy-hub-password'. |
| postgresql.auth.policyHubUser | string | `"hub"` | Non-root username for hub. |
| postgresql.auth.port | int | `5432` | Database port number |
| postgresql.enabled | bool | `true` | PostgreSQL chart configuration Switch to enable or disable the PostgreSQL helm chart |
| postgresql.fullnameOverride | string | `"policy-hub-postgresql"` | FullnameOverride to 'policy-hub-postgresql'. |
| postgresql.primary.extendedConfiguration | string | `""` | Extended PostgreSQL Primary configuration (increase of max_connections recommended - default is 100) |
| postgresql.primary.extraEnvVars[0].name | string | `"POLICY_HUB_PASSWORD"` |  |
| postgresql.primary.extraEnvVars[0].valueFrom.secretKeyRef.key | string | `"policy-hub-password"` |  |
| postgresql.primary.extraEnvVars[0].valueFrom.secretKeyRef.name | string | `"{{ .Values.auth.existingSecret }}"` |  |
| postgresql.primary.initdb.scriptsConfigMap | string | `"configmap-postgres-init"` |  |
| postgresql.readReplicas.extendedConfiguration | string | `""` | Extended PostgreSQL read only replicas configuration (increase of max_connections recommended - default is 100) |
| readinessProbe.failureThreshold | int | `3` |  |
| readinessProbe.initialDelaySeconds | int | `10` |  |
| readinessProbe.periodSeconds | int | `10` |  |
| readinessProbe.successThreshold | int | `1` |  |
| readinessProbe.timeoutSeconds | int | `1` |  |
| replicaCount | int | `3` |  |
| startupProbe | object | `{"failureThreshold":30,"initialDelaySeconds":10,"periodSeconds":10,"successThreshold":1,"timeoutSeconds":1}` | Following Catena-X Helm Best Practices, [reference](https://github.com/helm/charts/blob/master/stable/nginx-ingress/values.yaml#L210). |
| tolerations | list | `[]` | Tolerations for pod assignment |
| updateStrategy.rollingUpdate.maxSurge | int | `1` |  |
| updateStrategy.rollingUpdate.maxUnavailable | int | `0` |  |
| updateStrategy.type | string | `"RollingUpdate"` | Update strategy type, rolling update configuration parameters, [reference](https://kubernetes.io/docs/concepts/workloads/controllers/statefulset/#update-strategies). |

Autogenerated with [helm docs](https://github.com/norwoodj/helm-docs)
