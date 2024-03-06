# Security Assessment Policy-Hub 

|                           |                                                                                                |
| ------------------------- | ---------------------------------------------------------------------------------------------- |
| Contact for product       | [@evegufy](https://github.com/evegufy) <br> [@jjeroch](https://github.com/jjeroch)             |
| Security responsible      | [Szymon Kowalczyk](szymon.kowalczyk@zf.com) |
| Version number of product | 0.1.0                                                                                          |
| Dates of assessment       | 2024-02-16: Assessment                                                                      |
| Status of assessment      | Assessment Report                                                                            |

## Product Description

Policy-Hub project is an readonly REST API project, without implementation of an UI. (Pure Backend Component)

Main purpose of the product is to provide interested and authenticated C-X Users with read access to policy data, use cases, credential types, policy rules, that are created during an application seeding.

The Policy-Hub comprise the technical foundation for interaction, monitoring, auditing and further functionalities. 
They are state of the art in terms of technology portfolio, consist of open-source components whenever possible and are open-sourced themselves 100%.

Policy-Hub can be run anywhere: it can be deployed as a docker image, e. g. on Kubernetes (platform-independent, cloud, on prem or local).

The policy hub is using following key frameworks:

- .Net
- Entity Framework
[architecture & development concept](https://github.com/eclipse-tractusx/policy-hub/blob/main/docs/technical-documentation/architecture/Development%20Concept.md)

## Data Flow Diagram

```mermaid
flowchart LR

    CU(Company user)
    K("Keycloak (REST API)")
    PH(Policy Hub API)
    PHD[("Postgres Database \n \n (Data created with \n application seeding)")]

    subgraph centralidp[centralidp Keycloak]
     K
    end

    subgraph companyrealm[SharedIdP Keycloak or ownIdP]
     CU
    end

    subgraph Policy-Hub Product   
     PH
     PHD
    end

    K-->|"Authentication & Authorization Data \n (Using JWT)"|PH     
    CU-->|"Consumption of central, read-only REST API \n [HTTPS]"|PH
    PH-->|"Read policies, use cases, \n credential types, policy rules"|PHD
    CU-->|"IAM with OIDC \n [HTTPS]"|K
```

### Changes compared to last Security Assessment

N/A

### Features for Upcoming Versions

N/A

## Threats & Risks

All potential threats discussed during the assessment were already mitigated.

### Mitigated Threats

N/A 

### Performed Security Checks

- Static Application Security Testing (SAST) - CodeQL
- Dynamic Application Security Testing (DAST) - OWASP ZAP
- Secret Scanning - GitGuardian
- Software Composition Analysis (SCA) - SNYK
- Container Scan conducted - Trivy
- Infrastructure as Code - KICS
- Securing code, dependencies, containers, IaC and Cloud Deployments - SNYK

## NOTICE

This work is licensed under the [Apache-2.0](https://www.apache.org/licenses/LICENSE-2.0).

- SPDX-License-Identifier: Apache-2.0
- SPDX-FileCopyrightText: 2024 Contributors to the Eclipse Foundation
- Source URL: https://github.com/eclipse-tractusx/policy-hub
