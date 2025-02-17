//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the Rock.CodeGeneration project
//     Changes to this file will be lost when the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//

import { PublicAttributeBag } from "@Obsidian/ViewModels/Utility/publicAttributeBag";

/** Assessment View Model */
export type AssessmentBag = {
    /** Gets or sets the result data for the Assessment taken. */
    assessmentResultData?: string | null;

    /** Gets or sets the Id of the Rock.Model.AssessmentType */
    assessmentTypeId: number;

    /** Gets or sets the attributes. */
    attributes?: Record<string, PublicAttributeBag> | null;

    /** Gets or sets the attribute values. */
    attributeValues?: Record<string, string> | null;

    /** Gets or sets the date of when the Assessment was completed. */
    completedDateTime?: string | null;

    /** Gets or sets the created by person alias identifier. */
    createdByPersonAliasId?: number | null;

    /** Gets or sets the created date time. */
    createdDateTime?: string | null;

    /** Gets or sets the identifier key of this entity. */
    idKey?: string | null;

    /** Gets or sets the result last reminder date. */
    lastReminderDate?: string | null;

    /** Gets or sets the modified by person alias identifier. */
    modifiedByPersonAliasId?: number | null;

    /** Gets or sets the modified date time. */
    modifiedDateTime?: string | null;

    /** Gets or sets the Id of the person Rock.Model.Person who is associated with the assessment. */
    personAliasId: number;

    /** Gets or sets the date when the assessment was requested. */
    requestedDateTime?: string | null;

    /** Gets or sets the date of the requested due date. */
    requestedDueDate?: string | null;

    /** Gets or sets the RequesterPersonAliasId of the Rock.Model.Person that requested the assessment. */
    requesterPersonAliasId?: number | null;

    /**
     * Gets or sets the enum of the assessment status.
     * Requirement from Jon, a pending assessment will stay in a pending status if it was never taken, even if a new one is requested.
     */
    status: number;
};
