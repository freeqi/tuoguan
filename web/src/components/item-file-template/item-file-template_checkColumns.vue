<template>
  <div id="item-file">
    <Row class="main">
      <Col :sm="8" :md="6" :lg="4" class="aside">
        <Card dis-hover>
          <Tree v-if="!treeLoading" :data="treeData" @on-select-change="handleClickTreeNode"></Tree>
          <div v-else class="skeleton">
            <p class="skeleton-item"></p>
            <p class="skeleton-item"></p>
            <p class="skeleton-item"></p>
          </div>
        </Card>
      </Col>
      <Col :sm="16" :md="18" :lg="20" class="content">
        <div class="operate">
          <div>
            <Button type="primary" @click="add" v-permission="buttonRole.WPDA_XZDA">添加</Button>
            <sws-upload
              :importId="importId"
              v-permission="buttonRole.WPDA_DR"
              @on-success-upload="handleUploadSuccess"
            ></sws-upload>
            <Button v-if="[1,5].includes(this.warehHouseTypeId)" type="info" style="margin-left:10px;" @click="toggleAbnormal">{{showAbnormalFlag?'显示全部数据':'显示异常数据'}}</Button>
          </div>
          <Input
            v-model="searchKey"
            style="width: 350px"
            search
            enter-button
            @on-search="searchItem"
            placeholder="请输入关键字"
          />
        </div>
        <div class="table">
          <Checkbox-group v-model="tableColumnsChecked" @on-change="changeTableColumns">
            <span>采购价：</span>
            <Checkbox label="show">统一价</Checkbox>
            <Checkbox label="weak">巴南</Checkbox>
          </Checkbox-group>
          <Table :loading="loading" :columns="tableColumn2" :data="item_data"></Table>
          <div style="margin: 10px;" class="pagination" v-if="dataCount > 10">
            <div style="float: right;">
              <Page
                :total="dataCount"
                :page-size="pageSize"
                :current.sync="startPage"
                @on-change="changePage"
              ></Page>
            </div>
          </div>
        </div>
      </Col>
    </Row>

    <Modal v-model="delModal" width="400" class-name="vertical-center-modal">
      <p slot="header">
        <span>删除 {{itemName}}档案</span>
      </p>
      <div style="text-align:center; padding: 40px 0; font-size: 16px;">
        <p>
          <Icon type="ios-help-circle" color="#ff81a3" size="30" style="margin-right: 20px;"/>删除后不可恢复，您确定删除吗？
        </p>
      </div>
      <div slot="footer" class="center">
        <Button type="primary" @click="del">确定</Button>
        <Button type="default" @click="delModal=false">取消</Button>
      </div>
    </Modal>

    <!-- 物品档案调价modal -->
    <Modal v-model="editPriceModal" width="1040" :mask-closable="false" style="padding:0">
      <p slot="header" style="text-align: center;">{{this.$route.meta.title}}调价 {{itemName}}</p>
      <Button type="primary" style="margin-bottom:10px;" @click="addNewPrice">新增价格</Button>
      <div class="price_table">
        <Table
          :loading="loading"
          :columns="price_table_column"
          size="small"
          :data="price_data"
          height="350"
        >
          <template slot-scope="{row, index}" slot="centerName">
            <Select
              v-model="dialysisCenterId"
              placeholder="请选择透析中心"
              filterable
              v-if="editPriceIndex === index && row.centerId != 0"
            >
              <Option
                :value="item.id"
                v-for="item in centerList"
                :key="item.id"
              >{{item.dialysisName}}</Option>
            </Select>
            <span v-else>{{ row.centerName }}</span>
          </template>
          <template slot-scope="{row, index}" slot="purchasingPrice">
            <InputNumber
              :min="0"
              v-model="itemPrice.purchasingPrice"
              v-if="editPriceIndex === index"
            ></InputNumber>
            <span v-else>{{ row.purchasingPrice }}</span>
          </template>
          <template slot-scope="{row, index}" slot="retailPrice">
            <InputNumber :min="0" v-model="itemPrice.retailPrice" v-if="editPriceIndex === index"></InputNumber>
            <span v-else>{{ row.retailPrice }}</span>
          </template>
          <template slot-scope="{row, index}" slot="agreementPrice">
            <InputNumber
              :min="0"
              v-model="itemPrice.agreementPrice"
              v-if="editPriceIndex === index"
            ></InputNumber>
            <span v-else>{{ row.agreementPrice }}</span>
          </template>
          <template slot-scope="{row, index}" slot="referencePrice">
            <InputNumber
              :min="0"
              v-model="itemPrice.referencePrice"
              v-if="editPriceIndex === index"
            ></InputNumber>
            <span v-else>{{ row.referencePrice }}</span>
          </template>
          <template slot-scope="{row, index}" slot="gocGuidePrice">
            <InputNumber :min="0" v-model="itemPrice.gocGuidePrice" v-if="editPriceIndex === index"></InputNumber>
            <span v-else>{{ row.gocGuidePrice }}</span>
          </template>
          <template slot-scope="{row, index}" slot="socialSecurityPrice">
            <InputNumber
              :min="0"
              v-model="itemPrice.socialSecurityPrice"
              v-if="editPriceIndex === index"
            ></InputNumber>
            <span v-else>{{ row.socialSecurityPrice }}</span>
          </template>
          <template slot-scope="{row, index}" slot="action">
            <div v-if="editPriceIndex === index" class="button-group">
              <Button
                :loading="priceSaveLoading"
                size="small"
                type="primary"
                @click="handleSave(row,index)"
              >
                <span v-if="!priceSaveLoading">保存</span>
                <span v-else></span>
              </Button>
              <Button size="small" @click="editPriceCancel(row)" style="margin-left: 10px;">取消</Button>
            </div>
            <div v-else>
              <Tooltip content="修改" placement="top">
                <Icon
                  type="ios-create-outline"
                  size="22"
                  color="#4f95e8"
                  @click="handleEdit(row, index)"
                  style="cursor: pointer"
                ></Icon>
              </Tooltip>
            </div>
          </template>
        </Table>
      </div>
      <div slot="footer">
        <Button type="default" @click="editPriceModal=false;editPriceIndex = -1;price_data=[]">取消</Button>
      </div>
    </Modal>
  </div>
</template>

<script>
import swsUpload from '_c/sws-upload/'
const BUTTONROLE = {
  WPDA_DR: 'WPDA_DR',
  WPDA_XZDA: 'WPDA_XZDA'
}
export default {
  data () {
    return {
      tableColumnsChecked: ['show', 'weak'], // cy 勾选显示表单列
      table2ColumnList: {
        show: {
          title: '统一价',
          key: 'show',
          width: 70
        },
        weak: {
          title: '巴南',
          key: 'weak',
          width: 70
        }
      },
      tableColumn2: [],
      showAbnormalFlag: false, // cy 显示异常数据
      treeLoading: false,
      treeData: [],
      wareHouseCheckedId: 0, // 房类别
      loading: false,
      // cy：加载价格表单和保存的loading控制
      priceSaveLoading: false,

      startPage: 1,
      dataCount: 0,
      pageSize: 9,
      delModal: false,
      searchKey: '',
      item_data: [],
      itemId: -1, // 物品id
      itemName: '', // 物品名称

      // cy添加价格设置
      // editPriceModal: false,
      editPriceIndex: -1, // slot插槽显隐控制
      editPriceModal: false, // 调价modal
      centerList: [], // 透析中心列表
      dialysisCenterId: '', // 透析中心id
      dataState: 0, // 价格设置是否启用
      price_data: [], // 各个不同透析中心的对应item价格数据
      // 价格数据表头
      price_table_column: [
        {
          title: '机构',
          slot: 'centerName',
          align: 'left',
          width: 180
        },
        {
          title: '采购价',
          align: 'center',
          slot: 'purchasingPrice'
        },
        {
          title: '销售价',
          align: 'center',
          slot: 'retailPrice'
        },
        {
          title: '协议价',
          align: 'center',
          slot: 'agreementPrice'
        },
        {
          title: '参考价',
          align: 'center',
          slot: 'referencePrice'
        },
        {
          title: '政府指导价',
          align: 'center',
          slot: 'gocGuidePrice'
        },
        {
          title: '社保指导价',
          align: 'center',
          slot: 'socialSecurityPrice'
        },
        {
          title: '状态',
          align: 'center',
          key: 'dataState',
          render: (h, params) => {
            return h('i-switch', {
              props: {
                size: 'large',
                value: params.row.dataState === 1,
                disabled:
                  params.row.centerId === '0'
                    ? true
                    : this.editPriceIndex !== params.row._index
              },
              scopedSlots: {
                open: () => h('span', '启用'),
                close: () => h('span', '禁用')
              },
              on: {
                // 操作事件
                'on-change': value => {
                  // 1启用 2 禁用
                  params.row.dataState = value ? 1 : 2
                }
              }
            })
          }
        },
        {
          title: '操作',
          slot: 'action',
          align: 'center',
          width: 140
        }
      ],
      // cy:新增物品price
      itemPrice: {
        purchasingPrice: 0,
        agreementPrice: 0,
        retailPrice: 0,
        referencePrice: 0,
        gocGuidePrice: 0,
        socialSecurityPrice: 0
      },

      file: null,
      format: ['xls', 'xlsx'],
      buttonRole: BUTTONROLE
    }
  },
  props: {
    // 库类别
    warehHouseTypeId: {
      type: Number,
      default: -1
    },
    importId: {
      type: Number,
      required: true
    },
    tableColumn: {
      type: Array,
      default: () => {
        return []
      }
    },
    // 是否停止自动加载表格数据
    stopAutoLoad: {
      type: Boolean,
      default: false
    }
  },
  mounted () {
    this.changeTableColumns()
    this.$nextTick(() => {
      this.handleGetMenuList()
      !this.stopAutoLoad && this.getItemList()
    })
    // cy获取透析中心列表
    this.swsApi.swsGet(`CenterDialysis/DialysisDropList`).then(response => {
      if (response.data.success) {
        this.centerList = response.data.result
      } else {
        this.$Notice.error({
          title: '透析中心列表请求错误',
          desc: '网络错误，请稍后再试'
        })
      }
    })
  },
  methods: {
    // cy 获取显示列数据

    getTable2Columns () {
      let data = this.tableColumn.concat()
      this.tableColumnsChecked.forEach(item => { data.push(this.table2ColumnList[item]) })
      return data
    },
    // cy 改变表单显示列
    changeTableColumns () {
      this.tableColumn2 = this.getTable2Columns()
    },

    // cy 显示异常数据（销售价大于医保指导价）
    toggleAbnormal () {
      this.showAbnormalFlag = !this.showAbnormalFlag
      this.getItemList()
    },
    // 调价
    showEditPrice (item) {
      this.readonly = false
      this.saveModal = true
      this.editPriceModal = true
      // 保存当前物品id,名称
      this.itemId = item.id
      this.itemName = item.medicalItemName
      // cy:获取该物品的所有透析中心的价格详细信息（6个价格，状态）
      this.getPrice(item.id)
    },
    // cy:获取该物品的所有透析中心的价格详细信息（6个价格，状态）
    getPrice (id) {
      this.loading = true
      this.swsApi
        .swsGet(`Data/MedicalItemRecord/GetPrice/${id}`)
        .then(response => {
          this.loading = false
          if (response.data.success) {
            this.price_data = response.data.result
            // this.itemName = response.data.result[0].medicalItemRecordName
          } else {
            console.log(response.error)
          }
        })
    },
    // cy取消调价保存
    editPriceCancel (row) {
      // 取消slot的显示
      this.editPriceIndex = -1
      // 点击取消时，删除数组里新添加的空数据
      if (!row.hasOwnProperty('centerId')) {
        this.price_data.pop()
      }
    },
    // cy 保存 调价的新增/修改
    handleSave (row, index) {
      let params = {
        ...this.itemPrice,
        id: row.id ? row.id : 0,
        medicalId: this.itemId,
        centerId: this.dialysisCenterId,
        dataState: row.dataState
      }
      if (params.id === 0 && params.centerId === undefined) {
        // console.log(params.centerId)
        this.$Modal.warning({
          title: '提示',
          content: '请选择您需要设置的透析中心'
        })
      } else {
        this.priceSaveLoading = true
        this.swsApi
          .swsPost('Data/MedicalItemRecord/CreateUpdatePrice', params)
          .then(res => {
            // console.dir(res)
            this.priceSaveLoading = false
            if (res.data.success) {
              this.$Message.success(`调价操作成功！`)
              this.getPrice(this.itemId)
            } else {
              this.$Message.error(`调价操作失败，请稍后再试！`)
            }
            this.editPriceIndex = -1
          })
          .catch(e => {
            this.editPriceIndex = -1
            console.log(e)
          })
      }
    },
    // cy点击 调价触发slot
    handleEdit (row, index) {
      this.editPriceIndex = index
      this.dataState = row.dataState
      this.dialysisCenterId = row.centerId
      this.itemPrice.purchasingPrice = row.purchasingPrice
      this.itemPrice.retailPrice = row.retailPrice
      this.itemPrice.agreementPrice = row.agreementPrice
      this.itemPrice.referencePrice = row.referencePrice
      this.itemPrice.gocGuidePrice = row.gocGuidePrice
      this.itemPrice.socialSecurityPrice = row.socialSecurityPrice
    },
    // cy添加物品对应透析中心的价格
    addNewPrice () {
      this.price_data.push({
        centerName: '',
        purchasingPrice: 0,
        retailPrice: 0,
        agreementPrice: 0,
        referencePrice: 0,
        gocGuidePrice: 0,
        socialSecurityPrice: 0,
        dataState: 1
      })
      let index = this.price_data.length - 1
      this.handleEdit(this.price_data[index], index)
    },
    // emit
    add () {
      this.$emit('show-add-modal')
    },
    // 获取库房
    handleGetMenuList () {
      this.treeLoading = true
      this.swsApi
        .swsPost(`Data/WarehouseCatalog/tree/${this.warehHouseTypeId}`)
        .then(res => {
          this.treeData = res.data.result

          let reg = new RegExp('title', 'g')
          let reg1 = new RegExp('id', 'g')
          let cas = JSON.stringify(this.treeData)
            .replace(reg, 'label')
            .replace(reg1, 'value')
          let cascaderData = JSON.parse(cas)

          this.$emit('get-cascader', cascaderData)
          this.treeLoading = false
        })
        .catch(e => {
          this.treeLoading = false
          console.log(e)
        })
    },
    handleClickTreeNode (e) {
      if (!e.length) return
      let id = e[0].id
      this.wareHouseCheckedId = id
      this.searchKey = ''
      this.startPage = 1
      this.getItemList(this.startPage)
    },
    getItemList (i = 1) {
      let pageNum = i
      let params = {
        wareHouseId: this.wareHouseCheckedId,
        medicalItemType: this.warehHouseTypeId,
        name: this.searchKey,
        pageSize: this.pageSize,
        pageNum: pageNum,
        isAbnormal: this.showAbnormalFlag
      }
      this.loading = true
      this.swsApi.swsPost('Data/MedicalItemRecord/list', params).then(res => {
        if (res.data.success) {
          let data = res.data
          this.loading = false
          this.dataCount = data.dataCount
          this.item_data = data.result || []
        }
      })
    },
    changePage (i) {
      this.getItemList(i)
    },
    del () {
      this.delModal = false
      this.swsApi
        .swsPost(`Data/MedicalItemRecord/del/${this.itemId}`)
        .then(res => {
          // console.log(res)
          if (res.data.error === null) {
            this.$Message.success('删除成功！')
            // this.startPage = 1
            this.getItemList(this.startPage)
          } else {
            this.$Message.error('删除失败，请稍后再试！')
          }
        })
    },
    searchItem () {
      this.startPage = 1
      this.getItemList()
    },
    handleUploadSuccess () {
      this.startPage = 1
      this.getItemList(this.startPage)
    }
  },
  components: {
    swsUpload
  }
}
</script>

<style scoped lang="less">
#item-file {
  height: 100%;
  background: #ffffff;
  overflow: hidden;
  .main {
    height: 100%;
  }
  .aside {
    padding: 20px !important;
    overflow-x: auto;
    /deep/ .ivu-card {
      overflow-x: auto;
      margin: 0 10px;
    }
    & + .content {
      height: 100%;
      padding-bottom: 20px;
      overflow-y: auto;
    }
  }
  /deep/ .ivu-tree .ivu-tree-children li .ivu-tree-title {
    font-size: 15px;
    color: #424242;
  }
  .operate {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-top: 20px;
    padding-right: 20px;
  }
  .table {
    margin-top: 20px;
    padding-right: 20px;
    .ivu-table-wrapper {
      // cy:ui建议加上边框
      // border: none !important;
      & /deep/ .ivu-table {
        &::before,
        &::after {
          display: none !important;
        }
      }
      & /deep/ .ivu-table tr {
        font-size: 14px;
        background: #f2f8ff;
        border-bottom: none;
      }
    }
  }
}
.form {
  position: relative;
}
.skeleton {
  &-item {
    background: #eee;
    height: 22px;
    margin-bottom: 8px;
    &:nth-child(1) {
      animation: loading 1s ease-in-out  infinite ;
    }
    &:nth-child(2) {
      animation: loading .6s ease-in-out infinite ;
    }
    &:nth-child(3) {
      animation: loading .8s ease-in-out infinite ;
    }
  }
}
@keyframes loading {
  0% {
    width: 20%
  }
  50% {
    width: 100%
  }
  to {
    width: 20%
  }
}
</style>
