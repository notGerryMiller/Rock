import { defineComponent, PropType } from "vue";
import { AttributeFieldDefinitionBag } from "@Obsidian/ViewModels/Core/Grid/attributeFieldDefinitionBag";

export default defineComponent({
    props: {
        attributes: {
            type: Array as PropType<AttributeFieldDefinitionBag[]>,
            default: []
        },

        __attributeColumns: {
            type: Boolean as PropType<boolean>,
            default: true
        }
    }
});
