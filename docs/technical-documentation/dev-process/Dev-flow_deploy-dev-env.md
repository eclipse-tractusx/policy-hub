# Dev flow with deployment to dev environment

```mermaid
flowchart LR
    subgraph local
    D(Developer)
    end
    subgraph eclipse-tractusx
        direction LR
        D -- PR* to main*--> PH(policy-hub**)
        click PH "https://github.com/eclipse-tractusx/policy-hub"
    end
    subgraph Argo CD - sync to k8s cluster
    PH -- auto-sync --> A(Argo CD dev)
    click A "https://argo.dev.demo.catena-x.net"
    end
```

Note\* Every pull request (PR) requires at least one approving review by a committer

Note\*\* Unit tests and Sonarcloud runs at pull request, Trivy and KICS scans at merge as well as daily and Veracode scan runs weekly

Note\*\* Trivy and KICS scans are scheduled to daily

## NOTICE

This work is licensed under the [Apache-2.0](https://www.apache.org/licenses/LICENSE-2.0).

- SPDX-License-Identifier: Apache-2.0
- SPDX-FileCopyrightText: 2023 Contributors to the Eclipse Foundation
- Source URL: https://github.com/eclipse-tractusx/policy-hub
