/********************************************************************************
 * Copyright (c) 2021, 2023 Contributors to the Eclipse Foundation
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

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Org.Eclipse.TractusX.PolicyHub.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class CPLP3330Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "hub");

            migrationBuilder.CreateTable(
                name: "attribute_keys",
                schema: "hub",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    label = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_attribute_keys", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "policy_kinds",
                schema: "hub",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    label = table.Column<string>(type: "text", nullable: false),
                    technical_enforced = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_policy_kinds", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "policy_types",
                schema: "hub",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    label = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_policy_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "use_cases",
                schema: "hub",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    label = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_use_cases", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "policies",
                schema: "hub",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    kind_id = table.Column<int>(type: "integer", nullable: false),
                    left_operand_value = table.Column<string>(type: "text", nullable: true),
                    technical_key = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    attribute_key_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_policies", x => x.id);
                    table.ForeignKey(
                        name: "fk_policies_attribute_keys_attribute_key_id",
                        column: x => x.attribute_key_id,
                        principalSchema: "hub",
                        principalTable: "attribute_keys",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_policies_policy_kinds_policy_kind_id",
                        column: x => x.kind_id,
                        principalSchema: "hub",
                        principalTable: "policy_kinds",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "policy_kind_configurations",
                schema: "hub",
                columns: table => new
                {
                    policy_kind_id = table.Column<int>(type: "integer", nullable: false),
                    right_operand_value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_policy_kind_configurations", x => x.policy_kind_id);
                    table.ForeignKey(
                        name: "fk_policy_kind_configurations_policy_kinds_policy_kind_id",
                        column: x => x.policy_kind_id,
                        principalSchema: "hub",
                        principalTable: "policy_kinds",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "policy_assigned_types",
                schema: "hub",
                columns: table => new
                {
                    policy_id = table.Column<Guid>(type: "uuid", nullable: false),
                    policy_type_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_policy_assigned_types", x => new { x.policy_id, x.policy_type_id });
                    table.ForeignKey(
                        name: "fk_policy_assigned_types_policies_policy_id",
                        column: x => x.policy_id,
                        principalSchema: "hub",
                        principalTable: "policies",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_policy_assigned_types_policy_types_policy_type_id",
                        column: x => x.policy_type_id,
                        principalSchema: "hub",
                        principalTable: "policy_types",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "policy_assigned_use_cases",
                schema: "hub",
                columns: table => new
                {
                    policy_id = table.Column<Guid>(type: "uuid", nullable: false),
                    use_case_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_policy_assigned_use_cases", x => new { x.policy_id, x.use_case_id });
                    table.ForeignKey(
                        name: "fk_policy_assigned_use_cases_policies_policy_id",
                        column: x => x.policy_id,
                        principalSchema: "hub",
                        principalTable: "policies",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_policy_assigned_use_cases_use_cases_use_case_id",
                        column: x => x.use_case_id,
                        principalSchema: "hub",
                        principalTable: "use_cases",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "policy_attributes",
                schema: "hub",
                columns: table => new
                {
                    policy_id = table.Column<Guid>(type: "uuid", nullable: false),
                    key = table.Column<int>(type: "integer", nullable: false),
                    attribute_value = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_policy_attributes", x => new { x.policy_id, x.key, x.attribute_value });
                    table.ForeignKey(
                        name: "fk_policy_attributes_attribute_keys_attribute_key_id",
                        column: x => x.key,
                        principalSchema: "hub",
                        principalTable: "attribute_keys",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_policy_attributes_policies_policy_id",
                        column: x => x.policy_id,
                        principalSchema: "hub",
                        principalTable: "policies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "hub",
                table: "attribute_keys",
                columns: new[] { "id", "label" },
                values: new object[,]
                {
                    { 1, "Regex" },
                    { 2, "Static" },
                    { 3, "DynamicValue" },
                    { 4, "Brands" },
                    { 5, "Version" }
                });

            migrationBuilder.InsertData(
                schema: "hub",
                table: "policy_kinds",
                columns: new[] { "id", "label", "technical_enforced" },
                values: new object[,]
                {
                    { 1, "BusinessPartnerNumber", true },
                    { 2, "Membership", true },
                    { 3, "Framework", true },
                    { 4, "Purpose", false },
                    { 5, "Dismantler", true }
                });

            migrationBuilder.InsertData(
                schema: "hub",
                table: "policy_types",
                columns: new[] { "id", "is_active", "label" },
                values: new object[,]
                {
                    { 1, true, "Access" },
                    { 2, true, "Usage" },
                    { 3, true, "Purpose" }
                });

            migrationBuilder.InsertData(
                schema: "hub",
                table: "use_cases",
                columns: new[] { "id", "is_active", "label" },
                values: new object[,]
                {
                    { 1, true, "Traceability" },
                    { 2, true, "Quality" },
                    { 3, true, "PCF" },
                    { 4, true, "Behavioraltwin" },
                    { 5, true, "Sustainability" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_policies_attribute_key_id",
                schema: "hub",
                table: "policies",
                column: "attribute_key_id");

            migrationBuilder.CreateIndex(
                name: "ix_policies_kind_id",
                schema: "hub",
                table: "policies",
                column: "kind_id");

            migrationBuilder.CreateIndex(
                name: "ix_policy_assigned_types_policy_type_id",
                schema: "hub",
                table: "policy_assigned_types",
                column: "policy_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_policy_assigned_use_cases_use_case_id",
                schema: "hub",
                table: "policy_assigned_use_cases",
                column: "use_case_id");

            migrationBuilder.CreateIndex(
                name: "ix_policy_attributes_key",
                schema: "hub",
                table: "policy_attributes",
                column: "key");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "policy_assigned_types",
                schema: "hub");

            migrationBuilder.DropTable(
                name: "policy_assigned_use_cases",
                schema: "hub");

            migrationBuilder.DropTable(
                name: "policy_attributes",
                schema: "hub");

            migrationBuilder.DropTable(
                name: "policy_kind_configurations",
                schema: "hub");

            migrationBuilder.DropTable(
                name: "policy_types",
                schema: "hub");

            migrationBuilder.DropTable(
                name: "use_cases",
                schema: "hub");

            migrationBuilder.DropTable(
                name: "policies",
                schema: "hub");

            migrationBuilder.DropTable(
                name: "attribute_keys",
                schema: "hub");

            migrationBuilder.DropTable(
                name: "policy_kinds",
                schema: "hub");
        }
    }
}
