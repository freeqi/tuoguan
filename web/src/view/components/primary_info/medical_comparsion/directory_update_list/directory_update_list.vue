<template>
  <div id="medical">
    <div class="content">
      <!-- top -->
      <div class="header">
        <div class="filter">
          <Input
            v-model="searchKey"
            search
            enter-button
            @on-search="searchCompany"
            placeholder="请输入关键字"
            style="width: 300px;"
          />
          <span style="color:red;display:inline-block;margin-left:10px;">注意：只展示7天内更新的目录信息</span>
        </div>
      </div>
      <div class="table">
        <Table
          :loading="loading"
          style="margin-top: 20px;"
          :columns="table_column"
          :data="company_data"
          height="610"
        ></Table>
        <div class="pagination" v-if="dataCount > 10">
          <div style="float: right;">
            <Page
              :total="dataCount"
              :page-size="pageSize"
              :current.sync="startPage"
              @on-change="changePage"
              show-total
            ></Page>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import Operate from '@/components/operate'
// const BusinessType = 27
const BusinessType = '27'
export default {
  data () {
    return {
      loading: false,
      pageSize: 15,
      startPage: 1,
      searchKey: '',
      dataCount: 0,
      company_data: [],
      table_column: [
        {
          title: '名称',
          key: '名称',
          minWidth: 140
        },
        {
          title: '商品名',
          key: '商品名',
          minWidth: 120
        },
        {
          title: '国家医保编码',
          key: '国家医保编码',
          minWidth:140
        },
        {
          title: '包装单位',
          key: '包装单位',
          width: 120,
          align:'center'
        },
        {
          title: '规格型号',
          key: '规格型号',
          width: 130
        },
        {
          title: '基准价格',
          key: '基准价格',
          width: 120
        },
        {
          title: '自付比例',
          key: '自付比例',
          width: 100
        },
        {
          title: '医保目录备注',
          key: '医保目录备注',
          width: 130
        },
        {
          title: '目录变更时间',
          key: '目录变更时间',
          width: 150,
          render:(h,params)=>{
            return h('div',params.row.目录变更时间?params.row.目录变更时间.substring(0,10)+' ' +params.row.目录变更时间.substring(11,19):'')
          }
        },
        {
          title: '本地更新时间',
          key: '本地更新时间',
          width: 150,
          render:(h,params)=>{
            return h('div',params.row.本地更新时间.substring(0,10)+' ' +params.row.本地更新时间.substring(11,19))
          }
        },
      ],
    }
  },
  props: {},
  mounted () {
    this.getCompanyList();
  },
  computed: {
  },
  methods: {
    changePage (i) {
      this.getCompanyList(i)
    },
    // 获取供应商列表
    getCompanyList (i) {
      i = i || 1
      let args = {
        queryName: this.searchKey,
        pageSize: this.pageSize,
        pageNum: i
      }
      this.loading = true
      this.swsApi.swsPost('Data/SI/UpdateReminder', args).then(res => {
        this.loading = false
        if (res.data.result) {
          this.company_data = res.data.result
          this.dataCount = res.data.dataCount
        } else {
          this.$Notice.error({
            title: '请求错误',
            desc: '网络错误，请稍后再试'
          })
        }
      })
      .catch(e => {
        this.loading = false
      })
    },
    searchCompany () {
      this.startPage = 1
      this.getCompanyList()
    },
  },
}
</script>

<style scoped lang="less">
#medical {
  position: relative;
  height: 100%;
  .content {
    padding: 0 20px 20px;
    width: 100%;
    height: 100%;
    overflow-y: auto;
    .header {
      padding: 20px;
      background: #ffffff;
      .btn-groups {
        & > * {
          margin-bottom: 20px;
        }
      }
      .filter {
        display: flex;
        justify-content: flex-start;
        align-items: center;
        .text {
          margin: 0 10px 0 20px;
          font-size: 14px;
          color: #999;
        }
      }
    }
    .table {
      margin-bottom: 20px;
    }
    // .ivu-table-wrapper {
    //   border: none !important;
    //   /deep/ .ivu-table-default,
    //   /deep/ .ivu-table-large {
    //     background: transparent;
    //   }
    //   & /deep/ .ivu-table-header {
    //     margin-bottom: 20px;
    //   }
    //   & /deep/ .ivu-table {
    //     &::before,
    //     &::after {
    //       display: none !important;
    //     }
    //   }
    //   & /deep/ .ivu-table th {
    //     font-size: 14px;
    //     background: #fff;
    //     border-bottom: none;
    //   }
    // }
    .pagination {
      padding: 10px;
      &::after {
        content: '';
        display: block;
        height: 0;
        visibility: hidden;
        clear: both;
      }
    }
  }
}
.color-gray {
  color: #999;
}
.demo-upload-list {
  display: inline-block;
  width: 60px;
  height: 60px;
  text-align: center;
  line-height: 60px;
  border: 1px solid transparent;
  border-radius: 4px;
  overflow: hidden;
  background: #fff;
  position: relative;
  box-shadow: 0 1px 1px rgba(0, 0, 0, 0.2);
  margin-right: 4px;
}
.demo-upload-list img {
  width: 100%;
  height: 100%;
}
.demo-upload-list-cover {
  display: none;
  position: absolute;
  top: 0;
  bottom: 0;
  left: 0;
  right: 0;
  background: rgba(0, 0, 0, 0.6);
}
.demo-upload-list:hover .demo-upload-list-cover {
  display: block;
}
.demo-upload-list-cover i {
  color: #fff;
  font-size: 20px;
  cursor: pointer;
  margin: 0 2px;
}
#supplier-detail {
  position: absolute;
  top: 0;
  right: 0;
  width: 500px;
  height: 100%;
  z-index: 10;
}
</style>

