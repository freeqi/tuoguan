<template>
  <div class="sws-table">
    <Table
      :size="$attrs.size"
      :columns="columns"
      :data="data"
      :loading="$attrs.loading"
      :row-class-name="$attrs['row-class-name']"
      ref="table"
    ></Table>
    <!-- 分页 -->
    <div class="pagination" v-if="pagination.total > pagination.pageSize">
      <Page
        :total="pagination.total"
        :page-size="pagination.pageSize || 10"
        :current.sync="pagination.current"
        @on-change="handleChangePage"
      />
    </div>
  </div>
</template>

<script>
export default {
  data () {
    return {}
  },
  props: {
    data: {
      type: Array,
      default: _ => {
        return []
      }
    },
    columns: {
      type: Array,
      required: true
    },
    pagination: {
      type: Object
    }
  },
  inheritAttrs: false,
  methods: {
    handleChangePage (page) {
      this.$emit('on-change', page)
    },
    exportCsv (obj) {
      // console.log(obj)
      this.$refs.table.exportCsv(obj)
    }
  },
  components: {}
}
</script>

<style scoped lang="less">
</style>
