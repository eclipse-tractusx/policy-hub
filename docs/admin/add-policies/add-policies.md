# Summary

The following documentation gives an overview of how to add additional data to the Policy Hub.

## Adding new Policies

To add new policies the following steps must be executed:

### Add Policy

To add new Attributes you need to add an entry to the [policies](/src/database/PolicyHub.Migrations/Seeder/Data/policies.json) Seeding file.

The entry should look like the following:

```json
  {
    "id": "uuid for the policy, this is needed in the process later on",
    "kind_id": "one of the policy kinds mentioned below (as number)",
    "technical_key": "the technical key which is used to query it later in the process",
    "description": "a description of the policy",
    "is_active": "either true if the attribute should be active or false if the attribute shouldn't be active",
    "attribute_key_id": "one of the attribute keys mentioned below (as number)",
  },
```

**PolicyKindId**

```c#
public enum PolicyKindId
{
    BusinessPartnerNumber = 1,
    Membership = 2,
    Framework = 3,
    Purpose = 4,
    Dismantler = 5,
    ContractReference = 6
}
```

**Attribute Keys**

```c#
public enum AttributeKeyId
{
    Regex = 1,
    Static = 2,
    DynamicValue = 3,
    Brands = 4,
    Version = 5
}
```

### Assign policy to type

The created policy must be assigned to at least one policy type. To assign the policy to a type you need to add an entry to the [policy_assigned_types](/src/database/PolicyHub.Migrations/Seeder/Data/policy_assigned_types.json) Seeding file.

The entry should look like the following:

```json
{
  "policy_id": "value of the id column of the newly added policy",
  "policy_type_id": "one of the policy type ids mentioned below (as number)",
  "is_active": "either true if the attribute should be active or false if the attribute shouldn't be active"
}
```

**PolicyTypeId**

```c#
public enum PolicyTypeId
{
    Access = 1,
    Usage = 2
}
```

### Assign policy to use case

The created policy must be assigned to at least one use case. To assign the policy to a use case you need to add an entry to the [policy_assigned_use_cases](/src/database/PolicyHub.Migrations/Seeder/Data/policy_assigned_use_cases.json) Seeding file.

The entry should look like the following:

```json
{
  "policy_id": "value of the id column of the newly added policy",
  "use_case_id": "one of the use case ids mentioned below (as number)",
  "is_active": "either true if the attribute should be active or false if the attribute shouldn't be active"
}
```

**UseCaseId**

```c#
public enum UseCaseId
{
    Traceability = 1,
    Quality = 2,
    PCF = 3,
    Behavioraltwin = 4,
    Businesspartner = 5,
    CircularEconomy = 6,
    DemandCapacity = 7
}
```

## Adding new Attributes

To add new Attributes you need to add an entry to the [policy_attributes](/src/database/PolicyHub.Migrations/Seeder/Data/policy_attributes.json) Seeding file.

The entry should look like the following:

```json
{
  "id": "uuid",
  "policy_id": "id of an policy you want to link the attribute to",
  "key": "one of the attribute keys mentioned above (as number)",
  "attribute_value": "the specific value of the attribute you want to set",
  "is_active": "either true if the attribute should be active or false if the attribute shouldn't be active"
}
```

Depending on the attribute key which is set the output will slightly change. A regex Attribute will check the set value of a policy if it matches the regex. A dynamic value will take the user input of the value field. If non is set by the user `{dynamicValue}` is taken. For `Static`, `Brands` and `Version` Attributes the value will just render the content of the `attribute_value` column.

To make a Attribute available it must be set to `is_active` = `true` and a link to a use case must be added. For that a new entry in the [policy_attribute_assigned_use_cases](/src/database/PolicyHub.Migrations/Seeder/Data/policy_attribute_assigned_use_cases.json) must be added. The entry should look like the following:

```json
{
  "attribute_id": "value of the id column of the newly added policy_attribute",
  "use_case_id": "one of the use case ids mentioned above (as number)",
  "is_active": "either true if the attribute should be active or false if the attribute shouldn't be active"
}
```

It is possible to link an attribute to multiple UseCases.

## NOTICE

This work is licensed under the [Apache-2.0](https://www.apache.org/licenses/LICENSE-2.0).

- SPDX-License-Identifier: Apache-2.0
- SPDX-FileCopyrightText: 2024 Contributors to the Eclipse Foundation
- Source URL: https://github.com/eclipse-tractusx/policy-hub
