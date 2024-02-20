# Security Assessment Policy-Hub 

|                           |                                                                                                |
| ------------------------- | ---------------------------------------------------------------------------------------------- |
| Contact for product       | [@evegufy](https://github.com/evegufy) <br> [@jjeroch](https://github.com/jjeroch)             |
| Security responsible      | [Szymon Kowalczyk](szymon.kowalczyk@zf.com) |
| Version number of product |                                                                                           |
| Dates of assessment       | 2024-02-16: Assessment                                                                      |
| Status of assessment      | ASSESSMENT DRAFT                                                                            |


## Product Description

To be updated

### Important Links

To be updated

## Data Flow Diagram

```mermaid
flowchart LR

    CU-Own(Company user)
    NC1("Potential new company (admin)")
    CU-Shared1(Company user)

    K("Keycloak (REST API)")

    PH(Policy Hub API)
    PHD[("Postgres Database \n \n (Data created with \nApplication deployment)")]

    subgraph operator[Operator IdP]
      subgraph centralidp[centralidp Keycloak]
      K
      end

    subgraph sharedidp[shared IdP Keycloak]
        subgraph companyrealm1[Company realm]
        NC1
        CU-Shared1
        end

      end
    

    subgraph ownIdP
        CU-Own
    end
    
    end
        NC1-->|"IAM with OIDC \n [HTTPS]"|K

        CU-Shared1-->|"IAM with OIDC \n [HTTPS]"|K

        CU-Own-->|"IAM with OIDC \n [HTTPS]"|K

    subgraph Policy-Hub Product

        PH-->|"Data Read \n (Policy Data, use cases, credential types, policy rules) \n [TCP 5432 ]"|PHD

    end

    K-->|"Authentication & Authorization Data \n (Using JWT)"|PH
            
        NC1-->|"Consumption of central, read-only REST API \n [HTTPS]"|PH

        CU-Shared1-->|"Consumption of central, read-only REST API \n [HTTPS]"|PH

        CU-Own-->|"Consumption of central, read-only REST API \n [HTTPS]"|PH
```

### Changes compared to last Security Assessment

N/A

### Features for Upcoming Versions

N/A

## Threats & Risks

All potential threats discussed during the assessment were already mitigated.

### Mitigated Threats

To be updated.

### Performed Security Checks

To be updated



