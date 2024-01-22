
# Database View

- [Database View](#database-view)
  - [Database Overview](#database-overview)
  - [Database Structure](#database-structure)
    - [Enum Value Tables](#enum-value-tables)
    - [Mapping Tables](#mapping-tables)
    - [Configuration Table](#configuration-table)
    - [Attribute Mapping](#attribute-mapping)
    - [Policy Information](#policy-information)
  - [NOTICE](#notice)

## Database Overview

```mermaid
erDiagram
    policies ||..|| policy_assigned_types : policy_id
    policies ||..|| policy_assigned_use_case : policy_id
    policies ||..|| policy_kinds : kind_id
    policies ||..|| attribute_keys : attribute_key_id
    policies ||..|| policy_attributes : policy_id
    policy_attributes ||..|| attribute_keys : key
    policies {
        uuid id PK
        integer kind_id FK
        text left_operand_value
        text technical_key
        text description
        boolean is_active
        integer attribute_key FK
    }
    policy_assigned_types ||..|| policy_types : policy_type_id
    policy_assigned_types {
        uuid policy_id FK
        integer policy_type_id FK
    }
    policy_types {
        integer id PK
        text label
        bool is_active
    }
    policy_assigned_use_case ||..|| use_cases : use_case_id
    policy_assigned_use_case {
        uuid policy_id FK
        integer use_case_id FK
    }
    use_cases{
        integer id PK
        text label
        bool is_active
    }
    policy_kinds ||..|| policy_kind_configuration : policy_kind_id
    policy_kinds {
        integer id PK
        text label
        boolean technical_enforced
    }
    policy_kind_configuration{
        integer policy_kind_id PK
        text right_operand_value
    }
    attribute_keys {
        uuid id PK
        text label
    }
    policy_attributes{
        integer policy_id PK
        integer key PK
        text attribute_value PK
        bool is_active
    }
```

## Database Structure

The database is organized into several key tables, each serving a specific purpose:

### Enum Value Tables

`attribute_keys`, `policy_kinds`, `policy_types`, and `use_cases` are tables designed to store enum values. They contain an id and label, derived from the backend enums.

### Mapping Tables

`policy_assigned_types` and `policy_assigned_use_cases` are used to map types and use cases to specific policies.

### Configuration Table

The `policy_kind_configurations` table is utilized to define specific right operand values for each policy_kind.

### Attribute Mapping

In the `policy_attributes` table, specific attributes are mapped to policies. This allows for multiple attributes to be assigned to a single policy.

### Policy Information

The `policies` table serves as the repository for comprehensive information about each policy.

## NOTICE

This work is licensed under the [Apache-2.0](https://www.apache.org/licenses/LICENSE-2.0).

- SPDX-License-Identifier: Apache-2.0
- SPDX-FileCopyrightText: 2021-2023 Contributors to the Eclipse Foundation
- Source URL: https://github.com/eclipse-tractusx/policy-hub
