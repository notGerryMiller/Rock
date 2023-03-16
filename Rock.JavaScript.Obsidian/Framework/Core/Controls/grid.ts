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

import { PropType } from "vue";
import { NumberFilterMethod } from "@Obsidian/Enums/Controls/Grid/numberFilterMethod";
import { IGridColumnFilter, GridColumnDefinition, IGridData, StandardFilterProps, StandardCellProps } from "@Obsidian/Types/Controls/grid";
import { deepEqual } from "@Obsidian/Utility/util";

// #region Standard Component Props

export const standardColumnProps = {
    name: {
        type: String as PropType<string>,
        default: ""
    },

    title: {
        type: String as PropType<string>,
        required: false
    },

    textValue: {
        type: Object as PropType<(((row: Record<string, unknown>, column: GridColumnDefinition) => string | number | undefined) | string)>,
        required: false
    },

    sortField: {
        type: String as PropType<string>,
        required: false
    },

    sortValue: {
        type: Object as PropType<(((row: Record<string, unknown>, column: GridColumnDefinition) => string | number | undefined) | string)>,
        required: false
    },

    filter: {
        type: Object as PropType<IGridColumnFilter>,
        required: false
    },

    filterValue: {
        type: Object as PropType<(((row: Record<string, unknown>, column: GridColumnDefinition) => string | number | undefined) | string)>,
        required: false
    }
};

export const standardCellProps: StandardCellProps = {
    column: {
        type: Object as PropType<GridColumnDefinition>,
        required: true
    },
    row: {
        type: Object as PropType<Record<string, unknown>>,
        required: true
    }
};

export const standardFilterProps: StandardFilterProps = {
    modelValue: {
        type: Object as PropType<unknown>,
        required: false
    },

    column: {
        type: Object as PropType<GridColumnDefinition>,
        required: true
    },

    rows: {
        type: Array as PropType<Record<string, unknown>[]>,
        required: true
    }
};

// #endregion

// #region Filter Matches Functions

export function textColumnFilterMatches(needle: unknown, haystack: unknown): boolean {
    if (typeof (needle) !== "string") {
        return false;
    }

    if (!needle) {
        return true;
    }

    const lowerNeedle = needle.toLowerCase();

    if (haystack && typeof (haystack) === "string") {
        return haystack.toLowerCase().includes(lowerNeedle);
    }

    return false;
}

export function pickExistingColumnFilterMatches(needle: unknown, haystack: unknown): boolean {
    if (!Array.isArray(needle)) {
        return false;
    }

    if (needle.length === 0) {
        return true;
    }

    return needle.some(n => deepEqual(n, haystack, true));
}

export function numberColumnFilterMatches(needle: unknown, haystack: unknown, column: GridColumnDefinition, gridData: IGridData): boolean {
    if (!needle || typeof needle !== "object") {
        return false;
    }

    // Allow undefined values and number values, but everything else is
    // considered a non-match.
    if (haystack !== undefined && typeof haystack !== "number") {
        return false;
    }

    if (needle["method"] === NumberFilterMethod.Equals) {
        return haystack === needle["value"];
    }
    else if (needle["method"] === NumberFilterMethod.DoesNotEqual) {
        return haystack !== needle["value"];
    }

    // All the remaining comparison types require a value.
    if (haystack === undefined) {
        return false;
    }

    if (needle["method"] === NumberFilterMethod.GreaterThan) {
        return haystack > needle["value"];
    }
    else if (needle["method"] === NumberFilterMethod.GreaterThanOrEqual) {
        return haystack >= needle["value"];
    }
    else if (needle["method"] === NumberFilterMethod.LessThan) {
        return haystack < needle["value"];
    }
    else if (needle["method"] === NumberFilterMethod.LessThanOrEqual) {
        return haystack <= needle["value"];
    }
    else if (needle["method"] === NumberFilterMethod.Between) {
        if (typeof needle["value"] !== "number" || typeof needle["secondValue"] !== "number") {
            return false;
        }

        return haystack >= needle["value"] && haystack <= needle["secondValue"];
    }
    else if (needle["method"] === NumberFilterMethod.TopN) {
        const nCount = needle["value"];

        if (typeof nCount !== "number" || nCount <= 0) {
            return false;
        }

        const cacheKey = `number-filter-${column.name}.top-${nCount}`;
        let topn = gridData.cache[cacheKey] as number | undefined;

        if (topn === undefined) {
            topn = calculateColumnTopNRowValue(gridData.rows, nCount, column);
            gridData.cache[cacheKey] = topn;
        }

        return haystack >= topn;
    }
    else if (needle["method"] === NumberFilterMethod.AboveAverage) {
        const cacheKey = `number-filter-${column.name}.average`;
        let average = gridData.cache[cacheKey] as number | undefined;

        if (average === undefined) {
            average = calculateColumnAverageValue(gridData.rows, column);
            gridData.cache[cacheKey] = average;
        }

        return haystack > average;
    }
    else if (needle["method"] === NumberFilterMethod.BelowAverage) {
        const cacheKey = `number-filter-${column.name}.average`;
        let average = gridData.cache[cacheKey] as number | undefined;

        if (average === undefined) {
            average = calculateColumnAverageValue(gridData.rows, column);
            gridData.cache[cacheKey] = average;
        }

        return haystack < average;
    }
    else {
        return false;
    }
}

// #endregion

// #region Functions

export function calculateColumnAverageValue(rows: Record<string, unknown>[], column: GridColumnDefinition): number {
    let count = 0;
    let total = 0;
    for (const row of rows) {
        const rowValue = column.filterValue(row, column);

        if (typeof rowValue === "number") {
            total += rowValue;
            count++;
        }
    }

    return count === 0 ? 0 : total / count;
}

export function calculateColumnTopNRowValue(rows: Record<string, unknown>[], rowCount: number, column: GridColumnDefinition): number {
    const values: number[] = [];

    for (const row of rows) {
        const rowValue = column.filterValue(row, column);

        if (typeof rowValue === "number") {
            values.push(rowValue);
        }
    }

    // Sort in descending order.
    values.sort((a, b) => b - a);

    if (rowCount <= values.length) {
        return values[rowCount - 1];
    }
    else {
        return values[values.length - 1];
    }
}

// #endregion
