/********************************************************************************
 * Copyright (c) 2023 Contributors to the Eclipse Foundation
 *
 * See the NOTICE file(s) distributed with this work for additional
 * information regarding copyright ownership.
 *
 * This program and the accompanying materials are made available under the
 * terms of the Apache License, Version 2.0 which is available at
 * https://www.apache.org/licenses/LICENSE-2.0.
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 *
 * SPDX-License-Identifier: Apache-2.0
 ********************************************************************************/

using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Org.Eclipse.TractusX.PolicyHub.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class _25policyseeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_policies_policy_kinds_policy_kind_id",
                schema: "policy-hub",
                table: "policies");

            migrationBuilder.DropForeignKey(
                name: "fk_policy_attributes_attribute_keys_attribute_key_id",
                schema: "policy-hub",
                table: "policy_attributes");

            migrationBuilder.DropPrimaryKey(
                name: "pk_policy_attributes",
                schema: "policy-hub",
                table: "policy_attributes");

            migrationBuilder.AddColumn<Guid>(
                name: "id",
                schema: "policy-hub",
                table: "policy_attributes",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddPrimaryKey(
                name: "pk_policy_attributes",
                schema: "policy-hub",
                table: "policy_attributes",
                column: "id");

            migrationBuilder.CreateTable(
                name: "policy_attribute_assigned_use_cases",
                schema: "policy-hub",
                columns: table => new
                {
                    attribute_id = table.Column<Guid>(type: "uuid", nullable: false),
                    use_case_id = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_policy_attribute_assigned_use_cases", x => new { x.attribute_id, x.use_case_id });
                    table.ForeignKey(
                        name: "fk_policy_attribute_assigned_use_cases_policy_attributes_attri",
                        column: x => x.attribute_id,
                        principalSchema: "policy-hub",
                        principalTable: "policy_attributes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_policy_attribute_assigned_use_cases_use_cases_use_case_id",
                        column: x => x.use_case_id,
                        principalSchema: "policy-hub",
                        principalTable: "use_cases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_policy_attributes_policy_id",
                schema: "policy-hub",
                table: "policy_attributes",
                column: "policy_id");

            migrationBuilder.CreateIndex(
                name: "ix_policy_attribute_assigned_use_cases_use_case_id",
                schema: "policy-hub",
                table: "policy_attribute_assigned_use_cases",
                column: "use_case_id");

            migrationBuilder.AddForeignKey(
                name: "fk_policies_policy_kinds_kind_id",
                schema: "policy-hub",
                table: "policies",
                column: "kind_id",
                principalSchema: "policy-hub",
                principalTable: "policy_kinds",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_policy_attributes_attribute_keys_key",
                schema: "policy-hub",
                table: "policy_attributes",
                column: "key",
                principalSchema: "policy-hub",
                principalTable: "attribute_keys",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_policies_policy_kinds_kind_id",
                schema: "policy-hub",
                table: "policies");

            migrationBuilder.DropForeignKey(
                name: "fk_policy_attributes_attribute_keys_key",
                schema: "policy-hub",
                table: "policy_attributes");

            migrationBuilder.DropTable(
                name: "policy_attribute_assigned_use_cases",
                schema: "policy-hub");

            migrationBuilder.DropPrimaryKey(
                name: "pk_policy_attributes",
                schema: "policy-hub",
                table: "policy_attributes");

            migrationBuilder.DropIndex(
                name: "ix_policy_attributes_policy_id",
                schema: "policy-hub",
                table: "policy_attributes");

            migrationBuilder.DropColumn(
                name: "id",
                schema: "policy-hub",
                table: "policy_attributes");

            migrationBuilder.AddPrimaryKey(
                name: "pk_policy_attributes",
                schema: "policy-hub",
                table: "policy_attributes",
                columns: new[] { "policy_id", "key", "attribute_value" });

            migrationBuilder.AddForeignKey(
                name: "fk_policies_policy_kinds_policy_kind_id",
                schema: "policy-hub",
                table: "policies",
                column: "kind_id",
                principalSchema: "policy-hub",
                principalTable: "policy_kinds",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_policy_attributes_attribute_keys_attribute_key_id",
                schema: "policy-hub",
                table: "policy_attributes",
                column: "key",
                principalSchema: "policy-hub",
                principalTable: "attribute_keys",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
