import { GridColumnDefinition } from "@Obsidian/Types/Controls/grid";
import { defineComponent, PropType } from "vue";

export default defineComponent({
    props: {
        column: {
            type: Object as PropType<GridColumnDefinition>,
            required: true
        },
        row: {
            type: Object as PropType<Record<string, unknown>>,
            required: true
        }
    },

    setup(props) {
        return () => props.column.field ? props.row[props.column.field] : "";
    }
});
