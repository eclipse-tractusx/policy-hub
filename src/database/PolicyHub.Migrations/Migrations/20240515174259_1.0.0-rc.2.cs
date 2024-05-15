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

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Org.Eclipse.TractusX.PolicyHub.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class _100rc2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "policy-hub",
                table: "use_cases",
                keyColumn: "id",
                keyValue: 5,
                column: "label",
                value: "Businesspartner");

            migrationBuilder.InsertData(
                schema: "policy-hub",
                table: "use_cases",
                columns: new[] { "id", "is_active", "label" },
                values: new object[,]
                {
                    { 6, true, "CircularEconomy" },
                    { 7, true, "DemandCapacity" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "policy-hub",
                table: "use_cases",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                schema: "policy-hub",
                table: "use_cases",
                keyColumn: "id",
                keyValue: 7);

            migrationBuilder.UpdateData(
                schema: "policy-hub",
                table: "use_cases",
                keyColumn: "id",
                keyValue: 5,
                column: "label",
                value: "Sustainability");
        }
    }
}
