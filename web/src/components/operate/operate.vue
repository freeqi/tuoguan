<template>
  <div class="operate-box">
    <Tooltip transfer v-if="showWatch" :content="textWatch" placement="top">
      <Icon style="cursor: pointer; margin-right: 5px" type="md-list" size="22" color="#4f95e8" @click="doWatch" />
    </Tooltip>
    <Tooltip transfer v-if="showUndo" v-permission="permissionUndo" :content="textUndo" placement="top">
      <Icon style="cursor: pointer; margin-right: 5px" type="md-undo" size="22" color="#4f95e8" @click="undo" />
    </Tooltip>
    <Tooltip transfer v-if="showEdit" :content="textEdit" v-permission="permissionEdit" placement="top">
      <Icon style="cursor: pointer; margin-right: 5px" type="ios-create-outline" size="22" color="#4f95e8" @click="doEdit" />
    </Tooltip>
    <Tooltip transfer v-if="showDelete" v-permission="permissionDelete" :content="textDelete" placement="top">
      <Icon style="cursor: pointer" type="md-close" size="22" color="#f00" @click="doDelete" />
    </Tooltip>
  </div>
</template>

<script>
export default {
  name: "operate",
  data() {
    return {};
  },
  props: {
    showUndo: {
      type: Boolean,
      default: true,
    },
    showWatch: {
      type: Boolean,
      default: true,
    },
    showEdit: {
      type: Boolean,
      default: true,
    },
    showDelete: {
      type: Boolean,
      default: true,
    },
    handleUndo: {
      type: Function,
      default() {},
    },
    handleEdit: {
      type: Function,
      default() {},
    },
    handleWatch: {
      type: Function,
      default() {},
    },
    handleDelete: {
      type: Function,
      default() {},
    },
    textUndo: {
      default: "撤销",
    },
    textEdit: {
      default: "编辑",
    },
    textWatch: {
      default: "详情",
    },
    textDelete: {
      default: "删除",
    },
    permissionUndo: {
      type: String,
      default: "",
    },
    permissionEdit: {
      type: String,
      default: "",
    },
    permissionDelete: {
      type: String,
      default: "",
    },
  },
  methods: {
    doWatch() {
      this.handleWatch();
    },
    doEdit() {
      this.handleEdit();
    },
    doDelete() {
      this.handleDelete();
    },
    undo() {
      this.handleUndo();
    },
  },
  components: {},
};
</script>

<style scoped lang="less">
</style>
