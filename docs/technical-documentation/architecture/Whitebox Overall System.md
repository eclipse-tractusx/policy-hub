# Whitebox Overall System

## Summary

In the following image you see the overall system overview of the Policy Hub

```mermaid
flowchart LR

    C(Customer)
    ING(Ingress)
    PH(Policy Hub API)
    PHD[("Postgres Database \n \n (Data created with \n application seeding)")]

    subgraph Policy-Hub Product   
     ING
     PH
     PHD
    end

    C-->|"Authentication & Authorization Data \n (Using JWT)"|ING
    ING-->|"Forward Request"|PH
    PH-->|"Read policies, use cases, \n credential types, policy rules"|PHD

```

## NOTICE

This work is licensed under the [Apache-2.0](https://www.apache.org/licenses/LICENSE-2.0).

- SPDX-License-Identifier: Apache-2.0
- SPDX-FileCopyrightText: 2024 Contributors to the Eclipse Foundation
- Source URL: https://github.com/eclipse-tractusx/policy-hub
