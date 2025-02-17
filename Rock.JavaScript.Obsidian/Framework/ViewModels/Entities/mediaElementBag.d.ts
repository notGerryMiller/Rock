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

/** MediaElement View Model */
export type MediaElementBag = {
    /** Gets or sets the attributes. */
    attributes?: Record<string, PublicAttributeBag> | null;

    /** Gets or sets the attribute values. */
    attributeValues?: Record<string, string> | null;

    /** Gets or sets the created by person alias identifier. */
    createdByPersonAliasId?: number | null;

    /** Gets or sets the created date time. */
    createdDateTime?: string | null;

    /** Gets or sets a description of the Element. */
    description?: string | null;

    /** Gets or set the duration in seconds of media element. */
    durationSeconds?: number | null;

    /**
     * Gets or sets the file data JSON content that will be stored in
     * the database.
     */
    fileDataJson?: string | null;

    /** Gets or sets the identifier key of this entity. */
    idKey?: string | null;

    /** Gets or sets the MediaFolderId of the Rock.Model.MediaFolder that this MediaElement belongs to. This property is required. */
    mediaFolderId: number;

    /** Gets or sets the custom provider metric data for this instance. */
    metricData?: string | null;

    /** Gets or sets the modified by person alias identifier. */
    modifiedByPersonAliasId?: number | null;

    /** Gets or sets the modified date time. */
    modifiedDateTime?: string | null;

    /** Gets or sets the Name of the Element. This property is required. */
    name?: string | null;

    /** Gets or sets the System.DateTime this instance was created on the provider. */
    sourceCreatedDateTime?: string | null;

    /** Gets or sets the custom provider data for this instance. */
    sourceData?: string | null;

    /** Gets or sets the provider's unique identifier for this instance. */
    sourceKey?: string | null;

    /** Gets or sets the System.DateTime this instance was modified on the provider. */
    sourceModifiedDateTime?: string | null;

    /**
     * Gets or sets the thumbnail data JSON content that will stored
     * in the database.
     */
    thumbnailDataJson?: string | null;
};
