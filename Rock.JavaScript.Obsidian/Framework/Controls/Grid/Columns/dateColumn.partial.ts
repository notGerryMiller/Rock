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

import { standardColumnProps } from "@Obsidian/Core/Controls/grid";
import { GridColumnDefinition } from "@Obsidian/Types/Controls/grid";
import { RockDateTime } from "@Obsidian/Utility/rockDateTime";
import { defineComponent, PropType, VNode } from "vue";
import DateColumnCell from "../Cells/dateCell.partial.obs";

export default defineComponent({
    props: {
        ...standardColumnProps,

        format: {
            type: Object as PropType<VNode>,
            default: DateColumnCell
        },

        quickFilterValue: {
            type: Object as PropType<((row: Record<string, unknown>, column: GridColumnDefinition) => string | undefined)>,
            default: (r: Record<string, unknown>, c: GridColumnDefinition) => {
                if (!c.field) {
                    return undefined;
                }

                return RockDateTime.parseISO(r[c.field] as string)?.toASPString("d");
            }
        }
    }
});
