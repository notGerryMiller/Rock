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

import Grid from "./Grid/grid.partial.obs";

import AttributeColumns from "./Grid/Columns/attributeColumns.partial";
import BadgeColumn from "./Grid/Columns/badgeColumn.partial";
import BooleanColumn from "./Grid/Columns/booleanColumn.partial";
import ColoredBadgeColumn from "./Grid/Columns/coloredBadgeColumn.partial";
import Column from "./Grid/Columns/column.partial";
import DateColumn from "./Grid/Columns/dateColumn.partial";
import NumberColumn from "./Grid/Columns/numberColumn.partial";

import BadgeCell from "./Grid/Cells/badgeCell.partial.obs";
import BooleanCell from "./Grid/Cells/booleanCell.partial.obs";
import ColoredBadgeCell from "./Grid/Cells/coloredBadgeCell.partial.obs";
import DateCell from "./Grid/Cells/dateCell.partial.obs";
import NumberCell from "./Grid/Cells/numberCell.partial.obs";
import TextCell from "./Grid/Cells/textCell.partial";

import NumberColumnFilter from "./Grid/Filters/numberColumnFilter.partial.obs";
import PickExistingColumnFilter from "./Grid/Filters/pickExistingColumnFilter.partial.obs";
import TextColumnFilter from "./Grid/Filters/textColumnFilter.partial.obs";

import { numberColumnFilterMatches, pickExistingColumnFilterMatches, textColumnFilterMatches } from "@Obsidian/Core/Controls/grid";
import { IGridColumnFilter } from "@Obsidian/Types/Controls/grid";

// Export main Grid component.
export default Grid;

// Export column components.
export {
    AttributeColumns,
    BadgeColumn,
    BooleanColumn,
    ColoredBadgeColumn,
    Column,
    DateColumn,
    NumberColumn
};

// Export cell components.
export {
    BadgeCell,
    BooleanCell,
    ColoredBadgeCell,
    DateCell,
    NumberCell,
    TextCell
};

// Export filter components.
export {
    NumberColumnFilter,
    PickExistingColumnFilter,
    TextColumnFilter
};

export const textColumnFilter: IGridColumnFilter = {
     component: TextColumnFilter,

     matches: textColumnFilterMatches
 };

export const pickExistingColumnFilter: IGridColumnFilter = {
    component: PickExistingColumnFilter,

    matches: pickExistingColumnFilterMatches
};

export const numberColumnFilter: IGridColumnFilter = {
    component: NumberColumnFilter,

    matches: numberColumnFilterMatches
};
