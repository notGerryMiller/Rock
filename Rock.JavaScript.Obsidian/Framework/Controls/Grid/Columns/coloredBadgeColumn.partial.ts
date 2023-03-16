import { standardColumnProps } from "@Obsidian/Core/Controls/grid";
import { defineComponent, PropType, VNode } from "vue";
import ColoredBadgeCell from "../Cells/coloredBadgeCell.partial.obs";

export default defineComponent({
    props: {
        ...standardColumnProps,

        format: {
            type: Object as PropType<VNode>,
            default: ColoredBadgeCell
        }
    }
});
