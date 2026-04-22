<template>
  <div class="approve_container message_container">
    <header class="tab_header header">
      <span
        v-for="item in tabList"
        :key="item.id"
        :class="{'active': tabActivedId == item.id}"
        @click="changeTab(item.id)"
      >{{item.txt}}</span>
    </header>
    <main class="approve_container_content">
      <div class="table-wrapper">
        <Table
          :data="tableData"
          :loading="tableLoading"
          :columns="columns"
        ></Table>
        <div class="pagination">
          <Page
            class="page"
            v-show="dataCount"
            :total="dataCount"
            show-total
            :current="startPage"
            :page-size="pageSize"
            @on-change="changePage"
          ></Page>
        </div>
      </div>
    </main>
  </div>
</template>

<script>
export default {
  data () {
    return {
      tabList: [
        {
          id: 1,
          txt: '全部'
        },
        {
          id: 2,
          txt: '待我审核'
        },
        {
          id: 3,
          txt: '已审核'
        }
      ],
      tabActivedId: 1,

      tableData: [{}],
      tableLoading: false,
      columns: [
        {
          title: '标题内容'
        },
        {
          title: '通知时间',
          width: 220
        },
        {
          title: '审核状态',
          width: 140
        },
        {
          title: '操作',
          width: 120,
          render: (h, params) => {
            return (
              <span class="color-primary">
                查看详情
              </span>
            )
          }
        }
      ],
      dataCount: 0,
      startPage: 1,
      pageSize: 10
    }
  },
  methods: {
    changeTab (id) {
      this.tabActivedId = id
    },
    changePage (page) {}
  },
  components: {

  }
}
</script>

<style scoped lang="less">
@import "../style.less";
</style>
