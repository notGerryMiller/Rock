import { standardColumnProps } from "@Obsidian/Core/Controls/grid";
import { defineComponent, PropType, VNode } from "vue";
import BadgeCell from "../Cells/badgeCell.partial.obs";

export default defineComponent({
    props: {
        ...standardColumnProps,

        format: {
            type: Object as PropType<VNode>,
            default: BadgeCell
        },

        classSource: {
            type: Object as PropType<Record<string, string>>,
            required: false
        },

        colorSource: {
            type: Object as PropType<Record<string, string>>,
            required: false
        }
    }
});
