# Authentication

The authentication process for the Policy Hub involves interaction with the central IAM (Identity and Access Management). The configuration for IAM can be customized either locally during development through secrets or within the chart for the Docker image.

Currently, the Policy Hub performs a basic validation by checking for a valid token in the request. However, it's important to note that no permission checks are conducted at this stage.

## NOTICE

This work is licensed under the [Apache-2.0](https://www.apache.org/licenses/LICENSE-2.0).

- SPDX-License-Identifier: Apache-2.0
- SPDX-FileCopyrightText: 2021-2023 Contributors to the Eclipse Foundation
- Source URL: https://github.com/eclipse-tractusx/policy-hub
