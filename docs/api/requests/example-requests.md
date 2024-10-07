# Example Requests

The policy hub provides 4 endpoints. All endpoints are readonly endpoints although one of them is a post endpoint.

## GET: Policy Attributes

The endpoint is accessible under `/api/policy-hub/policy-attributes` and can be used to receive all possible attribute types.

## GET: Policy Types

The endpoint is accessible under `/api/policy-hub/policy-types`. It can be used to receive all available policies with all their attributes.

It's possible to filter the results by one of the following or both attributes:

- Type
  - Access
  - Usage
  - Purpose
- UseCase
  - Traceability
  - Quality
  - PCF
  - Behavioraltwin
  - Sustainability

As an result you'll receive a list of policies:

```json
[
    {
        "technicalKey": "BusinessPartnerNumber",
        "type": [
            "Access",
            "Usage"
        ],
        "description": "The business partner number restriction can get used to define which exact business partners (based on BPNL) are allowed to view or negotiate the respective data offer. Please ensure that you add minimum one 16-digit BPNL Number in the rightOperand; wildcards are not supported.",
        "useCase": [
            "Traceability",
            "Quality",
            "PCF",
            "Behavioraltwin",
            "Sustainability"
        ],
        "attribute": [
            {
                "key": "Regex",
                "value": "^BPNL[\\w|\\d]{12}$"
            }
        ],
        "technicalEnforced": true
    },
    {
        "technicalKey": "Membership",
        "type": [
            "Access",
            "Usage"
        ],
        "description": "The membership credential can get used to ensure that only CX members are allowed to view or negotiate the respective data offer.",
        "useCase": [
            "Traceability",
            "Quality",
            "PCF",
            "Behavioraltwin",
            "Sustainability"
        ],
        "attribute": [
            {
                "key": "Static",
                "value": "active"
            }
        ],
        "technicalEnforced": true
    },
    {
        "technicalKey": "FrameworkAgreement.traceability",
        "type": [
            "Usage"
        ],
        "description": "With the Framework Credential, only those participants which have signed the respective framework agreement (general or via a specific version) are allowed to view or negotiate the respective data offer. Generic: \"rightOperand\": \"active\"; specific \"rightOperand\": \"active:{version}\"",
        "useCase": [
            "Traceability"
        ],
        "attribute": [
            {
                "key": "Version",
                "value": "1.0"
            },
            {
                "key": "Version",
                "value": "1.1"
            },
            {
                "key": "Version",
                "value": "1.2"
            }
        ],
        "technicalEnforced": true
    }
]
```

## GET PolicyContent

The endpoint is accessible under `/api/policy-hub/policy-content?useCase={useCase}&type={type}&credential={technicalKey}&operatorId={operator}&value={value}` and can be used to receive a concrete json file for a policy.

The parameter you need to set for this endpoint can be received from the [policy-types](#get-policy-types) endpoint. The useCase as well as value are optional parameters. If you try to receive the content for a policy which has the Regex Attribute a value needs to be passed which is matching the regex pattern.

A possible response can look like this:

```json
{
    "content": {
        "@context": [
            "https://www.w3.org/ns/odrl.jsonld",
            {
                "cx": "https://w3id.org/catenax/v0.0.1/ns/"
            }
        ],
        "@type": "Offer",
        "@id": "....",
        "permission": {
            "action": "use",
            "constraint": {
                "leftOperand": "FrameworkAgreement.traceability",
                "operator": "eq",
                "rightOperand": "@FrameworkAgreement.traceability-Version"
            }
        }
    },
    "attributes": [
        {
            "key": "@FrameworkAgreement.traceability-Version",
            "possibleValues": [
                "active:1.0",
                "active:1.1",
                "active:1.2"
            ]
        }
    ]
}
```

As you can see the policy content was requested with operatorId = Equals, since the FrameworkAgreement.traceability has multiple values the values of the rightOperand will be displayed in an attributes array with the respective key used to link the values.

## POST: Policy Content

The endpoint is accessible under `/api/policy-hub/policy-content` and can be used to receive a concrete json file for a policy with and / or linked constraints.

The body of the request needs to contain the policyType and constraintOperand, valid values for the contraintOperand are `And` or `Or`.

The constrains can be passed as an array, the content of the array is a combination of the technicalKey of a policy type and the operator either `Equals` or `In`.

Limitations: the technicalKey of a constraint must only be included once.

And example:

```json
{
    "policyType": "Usage",
    "constraintOperand": "And",
    "constraints": [
        {
            "key": "FrameworkAgreement.traceability",
            "operator": "Equals"
        },
        {
            "key": "companyRole.dismantler",
            "operator": "In"
        }
    ]
}

```

Or example:

```json
{
    "PolicyType": "Usage",
    "ConstraintOperand": "Or",
    "Constraints": [
        {
            "Key": "FrameworkAgreement.traceability",
            "Operator": "Equals"
        },
        {
            "Key": "companyRole.dismantler",
            "Operator": "In"
        }
    ]
}

```

Multiple constrains example:

```json
{
    "PolicyType": "Usage",
    "ConstraintOperand": "And",
    "Constraints": [
        {
            "Key": "FrameworkAgreement.traceability",
            "Operator": "Equals"
        },
        {
            "Key": "companyRole.dismantler",
            "Operator": "In"
        },
        {
            "Key": "BusinessPartnerNumber",
            "Operator": "Equals",
            "Value": "BPNL00000003CRHK"
        }
    ]
}

```

If you want to try it out, check the [postman collection](./policy-hub.postman_collection.json)

## NOTICE

This work is licensed under the [Apache-2.0](https://www.apache.org/licenses/LICENSE-2.0).

- SPDX-License-Identifier: Apache-2.0
- SPDX-FileCopyrightText: 2024 Contributors to the Eclipse Foundation
- Source URL: https://github.com/eclipse-tractusx/policy-hub
