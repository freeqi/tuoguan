<template>
  <div id="statistics">
    <financial-template
      :tabData="ListArr1"
      @on-change="chooseArr1"
      :defaultTabCheckedId="tabCheckedId"
      @on-initial-hospital="getHospital"
    >
      <div class="tab-content" slot="content">
        <div class="info">
          <span>
            机构
            <Select
              v-model="hospitalCheckedId"
              filterable
              placeholder
              @on-change="changeHospital"
              style="width: 200px;margin: 0 8px;">
              <Option
                v-for="item in hospitalList"
                :key="item.dialysisId"
                :value="item.dialysisId"
              >{{item.dialysisName}}</Option>
            </Select>
          </span>
          <span v-show="[1,4,5,6].includes(tabCheckedId)">
            类别
            <Select
              v-model="selectedItemTypeIndex"
              style="width:200px;margin: 0 8px;"
            >
              <Option
                v-for="item in itemTypeList"
                :value="item.index"
                :key="item.index"
              >{{ item.name }}</Option>
            </Select>
          </span>
          <span>
            筛选
            <Input v-model="goodsSearch" search enter-button style="width:200px;margin:0 8px;display: inline-table" placeholder="搜索名称/厂家..." @on-change="keySearch" @on-search="keySearch"/>
          </span>
          <span class="out">
            <Button
              v-show="tabCheckedId==5 && !notifyFlag"
              type="success"
              style="margin-right: 10px;"
              @click="changePriceNotify"
            >批量调价通知</Button>
            <Button
              type="primary"
              style="margin-right: 10px;"
              @click="chooseArr1(tabCheckedId, true)"
            >查询</Button>
            <Button
              type="info"
              v-permission="buttonRole.WZTJ_DC"
              style="margin-right: 10px;"
              @click="exportTable"
            >导出</Button>
            <Button
              type="primary"
              ghost
              v-permission="buttonRole.WZTJ_DY"
              style="margin-right: 10px;"
              @click="print"
            >打印</Button>
          </span>
        </div>
        <div class="info">
          <span v-show="![2,3,5].includes(tabCheckedId)">
            日期
            <DatePicker
              type="daterange"
              placeholder="请选择日期"
              style="width: 200px;margin: 0 8px;"
              @on-change="chooseDate"
            ></DatePicker>
          </span>
          <span v-show="[1].includes(tabCheckedId)">
            统计方式
            <Select v-model="GroupType"  style="width:172px;margin: 0 8px;" @on-change="changeType">
              <Option :value="0">合计</Option>
              <Option :value="1">按天</Option>
              <Option :value="2">按月</Option>
            </Select>
          </span>
          <span v-show="[4,6].includes(tabCheckedId)">
            供应商
            <Select
              v-model="selectedSupplierId"
              filterable
              style="width:186px;margin: 0 8px;"
            >
              <Option v-for="item in supplierList" :value="item.id" :key="item.id">{{ item.name }}</Option>
            </Select>
          </span>
          <span v-show="tabCheckedId===5">
            数据类型
            <RadioGroup v-model="dataType" @on-change="dataTypeChange" type="button" style="margin: 0 8px;">
              <Radio :label="1">全部</Radio>
              <Radio :label="2">超医保限价</Radio>
              <Radio :label="3">低于采购价</Radio>
            </RadioGroup>
          </span>
          <span v-show="tabCheckedId===5">
            医保
            <RadioGroup v-model="medicalDataType" @on-change="dataTypeChange" type="button" style="margin: 0 8px;">
              <Radio :label="1">全部</Radio>
              <Radio :label="2">未对照</Radio>
            </RadioGroup>
          </span>
          <h2 style="margin-top:20px;">{{subTitle}}</h2>
          <Table
            ref="statisticTable"
            class="default-table"
            :loading="table_loading"
            :columns="columns"
            :data="table_show_data"
            style="margin-top: 10px;"
            :height="tableHeight"
            @on-select-all="handleSelectAll"
            @on-select-all-cancel="handleSelectAll"
            @on-select="handleSelectRow"
            @on-select-cancel="handleCancelRow">
            <!-- <template slot-scope="{row, index}" slot="hiCenterCode" v-if="tabCheckedId===5&&[1,2].includes(selectedItemTypeIndex)">
              <span>{{row.hiCenterCode}}</span>
            </template> -->
          </Table>
          <div class="notify-btn-goups" v-show="notifyFlag">
            <Button type="primary" style="margin-right:10px;" @click="showNotifyModal">确定</Button>
            <Button style="margin-left:10px;" type="default" @click="notifyCancel">取消</Button>
            <span style="margin-left:10px;">已经选中：{{selectedSum}} 项</span>
          </div>
          <div class="pagination" v-if="dataCount > kcPageSize" >
            <Page
              :total="dataCount"
              show-total
              @on-change="changeKcPage"
              :page-size="kcPageSize"
              :current.sync="kcStartPage"
            />
          </div>
        </div>
      </div>
    </financial-template>

    <!-- 库存批量调价modal -->
    <Modal v-model="changePriceModal" width="1040" :mask-closable="false" style="padding:0">
      <p slot="header" style="text-align: center;">批量调价</p>
      <div class="price_table">
        <Table
          :columns="price_table_column"
          size="small"
          :data="price_data"
          height="370"
        >
          <template slot-scope="{row, index}" slot="salePrice">
            <InputNumber :min="0" v-model="row.salePrice" active-change @on-change="changePrice(row,index)"></InputNumber>
            <Icon type="md-close-circle" size="22" color="red" v-show="row.salePrice === null" style="margin-left:5px"></Icon>
          </template>
        </Table>
      </div>
      <div slot="footer">
        <span style="float:left;">
          调价截止日期：
          <DatePicker v-model="disposeTime" type="date" :options="options1" placeholder="请选择调价截止日期"></DatePicker>
        </span>
        <Button type="primary" @click="publish">发布通知</Button>
        <Button type="default" @click="changePriceModal=false;">取消</Button>
      </div>
    </Modal>
  </div>
</template>

<script>
import financialTemplate from '@/components/financial-template'
import { toFilterKey, formateDateToString, _debounce, accAdd } from '@/libs/tools.js'
import { mapMutations } from 'vuex'
const BUTTONROLE = {
  WZTJ_DC: 'WZTJ_DC',
  WZTJ_DY: 'WZTJ_DY'
}
export default {
  name: 'statistics',
  components: {
    financialTemplate
  },
  data () {
    return {
      GroupType:0,
      // 新增 库存统计 筛选后的数据的合计统计
      cx_totalData: {},
      emptyPriceList: new Set(), // 未填调价的数据
      options1: {
        disabledDate (date) {
          return date && date.valueOf() < Date.now() - 86400000
        }
      },
      disposeTime: '', // 调价截止日期
      changePriceModal: false, // 批量调价modal
      selectedIds: new Set(), // 选中的批量通知项id
      selectedItems: [], // 选中的批量通知项
      selectedSum: 0,
      notifyFlag: false, // 批量 通知
      dataType: 1, // 数据类型
      medicalDataType: 1, // 医保编码 类型筛选 （已对照和未对照）
      tableHeight: 0,
      goodsSearch: '', // 低库存 物品查询
      pageSize: 10000,
      selectedSupplierId: '', // 选中的供应商ID
      supplierList: [],
      selectedItemTypeIndex: '0',
      itemTypeList: [
        { index: '0', name: '全部' },
        { index: 1, name: '药品' },
        { index: 2, name: '耗材' },
        { index: 3, name: '固定资产' },
        { index: 4, name: '低值易耗' }
      ],
      hospitalCheckedId: 'abc3d60b474c4fef946a66887348c41a',
      hospitalCheckedName: '',
      hospitalList: [],

      ListArr1: [
        { childKey: 1, childValue: '用量统计表' },
        { childKey: 2, childValue: '低库存预警表' },
        { childKey: 3, childValue: '高积压库存统计表' },
        { childKey: 4, childValue: '供应商入库统计表' },
        { childKey: 5, childValue: '库存统计表' },
        { childKey: 6, childValue: '月均用量统计' }
      ],
      tabCheckedId: 2, //

      subTitle: '低库存预警表',
      dateData: [],

      columns: [],
      table_loading: false,
      table_data: [],
      table_warning_data: [], // 新增 保存 异常数据
      table_lowerSalePrice_data: [], // 保存 销售价低于采购价的数据
      table_show_data: [], // 新增 用于前端分页
      table_filter_data: [], // 过滤后的数据
      dataCount: 0, // 数据量
      kcPageSize: 14,
      kcStartPage: 1,
      price_data: [], // 被选中的 批量调价 数据
      dataDetail: [],
      price_table_column: [
        { type: 'index', width: 60, title: '序号', align: 'center' },
        { title: '名称', key: 'medicalName', minWidth: 120 },
        { title: '规格', key: 'specifications', minWidth: 120 },
        { title: '采购价', key: 'purchasingPrice', align: 'center', minWidth: 67 },
        { title: '销售价',
          key: 'upSalePrice',
          align: 'center',
          minWidth: 67,
          render: (h, params) => {
            return params.row.dataType === 2 ? <span class="dataWarning" style="color:red;">{params.row.upSalePrice}</span> : <span>{params.row.upSalePrice}</span>
          }
        },
        { title: '双控价', key: 'dualPrice', align: 'center', minWidth: 67 },
        { title: '医保限价', key: 'socialSecurityPrice', align: 'center', minWidth: 67 },
        { title: '机构', key: 'dialysisName', tooltip: true, minWidth: 120 },
        { title: '厂家', key: 'manufacturer', tooltip: true, minWidth: 120 },
        { title: '供应商', key: 'supplierName', tooltip: true, minWidth: 120 },
        { title: '调整价格',
          key: 'salePrice',
          slot: 'salePrice',
          minWidth: 110
        }
      ], // 批量调价 table columns
      column1: [
        // { type: 'index', width: 40, title: '序号', align: 'center' },
        {
          title: '序列',
          key: 'no',
          align: 'center',
          width: 60,
          render: (h, {index}) => {
            let No = (this.kcStartPage - 1) * this.kcPageSize + index + 1
            return <span>{No}</span>
          }
        },
        { title: '名称', key: 'medicalItemName', minWidth: 130 },
        { title: '机构名称', key: 'dialysisName', width: 180 },
        { title: '批次号', key: 'batchNo', width: 120 },
        { title: '类型', key: 'itemTypeName', width: 80 },
        { title: '规格', key: 'specifications', minWidth: 120 },
        { title: '单位', key: 'unitName', width: 60, align: 'center' },
        { title: '厂家', key: 'manufacturer', minWidth: 120, tooltip: true },
        { title: '成本单价', key: 'inPrice', minWidth: 100, align: 'center' },
        { title: '成本总价', key: 'totalInPrice', minWidth: 100, align: 'center' },
        { title: '销售单价', key: 'salePrice', minWidth: 100, align: 'center' },
        { title: '销售总价', key: 'totalSalePrice', minWidth: 100, align: 'center' },
        { title: '总数量', key: 'totalInQty', width: 80, align: 'center' }
      ],
      column2: [
        // { type: 'index', width: 40, title: '序号', align: 'center' },
        {
          title: '序列',
          key: 'no',
          align: 'center',
          width: 60,
          render: (h, {index}) => {
            let No = (this.kcStartPage - 1) * this.kcPageSize + index + 1
            return <span>{No}</span>
          }
        },
        { title: '名称', key: 'medicalItemName', minWidth: 120 },
        { title: '机构名称', key: 'dialysisName', width: 180 },
        { title: '类型', key: 'itemTypeName', width: 120, align: 'center' },
        { title: '规格', key: 'specifications', minWidth: 120 },
        { title: '单位', key: 'specificationsUnitName', width: 80, align: 'center' },
        { title: '警戒库存', key: 'minInventory', align: 'center', width: 80 },
        { title: '剩余库存', key: 'inventory', align: 'center', width: 80 },
        { title: '厂家', key: 'manufacturer', tooltip: true, minWidth: 140 }
      ],
      column21: [
        { title: '机构名称', key: 'dialysisName' },
        { title: '药品名称', key: 'name' },
        { title: '库存数量', key: 'name' },
        { title: '最低库存数量', key: 'name' }
      ],
      column3: [
        // { type: 'index', width: 40, title: '序号', align: 'center' },
        {
          title: '序列',
          key: 'no',
          align: 'center',
          width: 60,
          render: (h, {index}) => {
            let No = (this.kcStartPage - 1) * this.kcPageSize + index + 1
            return <span>{No}</span>
          }
        },
        { title: '名称', key: 'medicalItemName' },
        { title: '机构名称', key: 'dialysisName' },
        { title: '类型', key: 'itemTypeName', width: 65 },
        { title: '数量', key: 'inQty', width: 65 },
        { title: '规格', key: 'specifications' },
        { title: '单位', key: 'specificationsUnitName', width: 65 },
        { title: '厂家', key: 'manufacturer' }
      ],
      column4: [
        // { type: 'index', width: 40, title: '序号', align: 'center' },
        {
          title: '序列',
          key: 'no',
          align: 'center',
          width: 60,
          render: (h, {index}) => {
            let No = (this.kcStartPage - 1) * this.kcPageSize + index + 1
            return <span>{No}</span>
          }
        },
        { title: '名称', key: 'medicalItemName', minWidth: 120 },
        { title: '机构名称', key: 'dialysisName', minWidth: 120 },
        { title: '类型', key: 'itemTypeName', width: 80 },
        { title: '规格', key: 'procurementPackage', minWidth: 120 },
        { title: '单位', key: 'unitName', width: 60, align: 'center' },
        { title: '入库总量', key: 'inQty', minWidth: 80, align: 'center' },
        { title: '厂家', key: 'manufacturer', tooltip: true, minWidth: 120 },
        { title: '供应商名称', key: 'supplierName', tooltip: true, minWidth: 120 }
      ],
      column5: [
        // { type: 'index', width: 40, title: '序号', align: 'center' },
        {
          title: '序列',
          key: 'no',
          align: 'center',
          width: 60,
          render: (h, {index}) => {
            let No = (this.kcStartPage - 1) * this.kcPageSize + index + 1
            return <span>{No}</span>
          }
        },
        { title: '名称', key: 'medicalItemName', minWidth: 120 },
        { title: '物品编码', key: 'medicalItemCode', minWidth: 95 },
        // { title: '医保编码', key: 'hiCenterCode', slot: 'hiCenterCode', minWidth: 80 },
        { title: '批号', key: 'batchNo', minWidth: 100 },
        { title: '包装', key: 'packaging', minWidth: 100 },
        { title: '规格', key: 'specifications', minWidth: 120 },
        { title: '单位', key: 'unitName', width: 60, align: 'center' },
        { title: '上月用量', key: 'monthAverage', align: 'center', minWidth: 80 },
        { title: '库存', key: 'inQty', align: 'center', minWidth: 80 },
        { title: '警戒库存', key: 'minInventory', width: 90, align: 'center' },
        { title: '采购价', key: 'inPrice', align: 'center', minWidth: 80 },
        { title: '采购金额', key: 'purchaseAmount', align: 'center', minWidth: 90 },
        { title: '销售价',
          key: 'salePrice',
          align: 'center',
          minWidth: 80,
          render: (h, params) => {
            return params.row.dataType === 2 ? <span class="dataWarning" style="color:red;">{params.row.salePrice}</span> : <span>{params.row.salePrice}</span>
          }
        },
        { title: '双控价', key: 'dualPrice', align: 'center', minWidth: 67 },
        { title: '医保限价', key: 'socialSecurityPrice', align: 'center', minWidth: 95 },
        { title: '医保系统规格', key: 'ypgg', align: 'center', minWidth: 95 },
        // { title: '采购数量', key: 'inQty' },
        // { title: '保质期至', key: 'qualityDate' },
        // { title: '生产日期', key: 'productionDate' },
        // { title: '保质期至', key: 'qualityDate' },
        {
          title: '生产日期',
          key: 'productionDate',
          align: 'center',
          width: 90,
          render: (h, params) => {
            return (
              <span>
                {params.row.productionDate && formateDateToString(
                  new Date(params.row.productionDate),
                  'yyyy-MM-dd'
                )}
              </span>
            )
          }
        },
        {
          title: '有效期至',
          key: 'qualityDate',
          align: 'center',
          width: 90,
          render: (h, params) => {
            return (
              <span>
                {params.row.qualityDate && formateDateToString(
                  new Date(params.row.qualityDate),
                  'yyyy-MM-dd'
                )}
              </span>
            )
          }
        },
        // { title: 'isWarning', key: 'isWarning' },
        { title: '机构', key: 'dialysisName', tooltip: true, minWidth: 120 },
        { title: '厂家', key: 'manufacturer', tooltip: true, minWidth: 120 },
        { title: '供应商', key: 'supplierName', tooltip: true, minWidth: 120 }
      ],
      column6: [
        {
          title: '序列',
          key: 'no',
          align: 'center',
          width: 60,
          render: (h, {index}) => {
            let No = (this.kcStartPage - 1) * this.kcPageSize + index + 1
            return <span>{No}</span>
          }
        },
        { title: '机构名称', key: 'dialysisName', minWidth: 130 },
        { title: '类型', key: 'itemTypeName', width: 60 },
        { title: '名称', key: 'medicalItemName', minWidth: 100 },
        { title: '规格', key: 'specifications', minWidth: 120 },
        { title: '厂家', key: 'manufacturer', minWidth: 130 },
        { title: '单位', key: 'specificationsUnitName', width: 60 },
        { title: '总用量', key: 'totalInQty', minWidth: 60, align: 'center' },
        { title: '月均用量', key: 'monthInQty', minWidth: 60, align: 'center' },
        { title: '成本单价', key: 'inPrice', minWidth: 60, align: 'center' },
        { title: '月均成本总额', key: 'monthInMoney', minWidth: 80, align: 'center' },
        // { title: '规格单位', key: 'specificationsUnitName', width: 80, align: 'center' },
      ],
      buttonRole: BUTTONROLE,
      printDatas: {
        api: '',
        title: '',
        columns: [],
        params: {}
      }
    }
  },
  mounted () {
    let args = {
      pageSize: 10000
    }
    this.swsApi
      .swsPost('Data/Supplier/list', args)
      .then(res => {
        if (res.data.success) {
          this.supplierList = [{ name: '全部', id: '' }, ...res.data.result]
        } else {
          this.$Notice.error({
            title: '请求错误',
            desc: '获取供应商列表错误，请稍后再试'
          })
        }
      })
      .catch(e => {
        this.$Notice.error({
          title: '请求错误',
          desc: '获取供应商列表错误，请稍后再试'
        })
      })

    // cy 调整table高度
    // this.tableHeight = document.documentElement.clientHeight - 400
  },
  methods: {
    ...mapMutations(['setColumns', 'setTitle', 'setParams', 'setApi']),
    changeType(val){
      this.table_show_data = [];
      if(val!==0){
        if(this.column1[3].key!='outDateValue'){
          this.column1.splice(3,0, { title: '时间', key:'outDateValue', width:120,align:'center' })
        }
      }else{
        this.column1.splice(3,1);
      }
    },
    handleInputNumber (row, index) {
      // console.log('handleInputNumber', index, row)
      this.price_data.splice(index, 0)
      this.price_data.splice(index, 1, row)
    },
    changePrice (row, index) {
      // console.log('changePrice', index, row)
      if (row.salePrice === null) {
        this.emptyPriceList.add(row.id)
        return false
      }
      if (this.emptyPriceList.has(row.id)) {
        this.emptyPriceList.delete(row.id)
      }
      // 此处加了防抖
      _debounce(this.handleInputNumber, [row, index], 500)
    },
    keySearch () {
      this.setData()
      setTimeout(() => {
        this.setChecked()
      }, 0)
    },
    publish () {
      let size = this.emptyPriceList.size
      if (size > 0) {
        this.$Message.warning(`请完善调价信息，共有 ${size} 处未填写。`)
        return false
      }
      if (this.disposeTime === '') {
        this.$Message.warning(`请填写调价截止日期。`)
        return false
      }
      let params = {
        inputs: this.price_data,
        outTime: formateDateToString(this.disposeTime, 'yyyy-MM-dd')
      }
      this.swsApi.swsPost(`Information/Information/BatchAdd`, params)
        .then(res => {
          if (res.data.success) {
            this.$Message.success(`通知发送成功。`)
            this.changePriceModal = false
            this.notifyCancel()
          } else {
            this.$Notice.warning({
              title: '请求错误',
              desc: `批量通知发送错误，${res.data.error}`,
              duration: 0
            })
          }
        })
        .catch(e => {
          this.$Notice.error({
            title: '请求错误',
            desc: `请求错误，${e}`,
            duration: 0
          })
        })
      // console.log('publish', this.price_data, params)
    },
    // cy 给跨页丢失的选中行重新添加选中/禁用状态
    setChecked () {
      // 当前页的table数据
      let objData = this.$refs.statisticTable.objData
      for (let index in objData) {
        // cy 根据保存的已勾选id来设置勾选状态
        if (this.selectedIds.has(objData[index].id)) {
          objData[index]._isChecked = true
          // objData[index]._isHover = false
        }
        if (objData[index].id === null) {
          objData[index]._disabled = true
        }
      }
    },
    // cy 全选和取消全选时触发
    handleSelectAll (selection) {
      if (selection.length === 0) {
        // cy 若取消全选，删除保存在selectedIds里和当前table数据的id一致的数据，达到，当前页取消全选的效果
        // 当前页的table数据
        let data = this.$refs.statisticTable.data
        data.forEach(item => {
          if (this.selectedIds.has(item.id)) {
            this.selectedIds.delete(item.id)
            this.deleteItem(item)
          }
        })
      } else {
        selection.forEach(item => {
          if (!this.selectedIds.has(item.id)) {
            this.selectedIds.add(item.id)
            this.addItem(item)
          }
        })
      }
      this.selectedSum = this.selectedIds.size
    },
    // cy 选中某一行
    handleSelectRow (selection, row) {
      this.selectedIds.add(row.id)
      this.selectedSum !== this.selectedIds.size && this.addItem(row)
      this.selectedSum = this.selectedIds.size
    },
    // cy 取消某一行
    handleCancelRow (selection, row) {
      this.selectedIds.delete(row.id)
      this.selectedSum !== this.selectedIds.size && this.deleteItem(row)
      this.selectedSum = this.selectedIds.size
    },
    addItem (item) {
      this.selectedItems.push(item)
    },
    deleteItem (item) {
      this.selectedItems = this.selectedItems.filter(res => {
        return res.id !== item.id
      })
    },
    clearSelectedIds () {
      // 清空ids集合
      this.selectedIds.clear()
      this.selectedSum = 0
    },
    showNotifyModal () {
      if (this.selectedItems.length === 0 ) {
        this.$Message.warning('请至少选择一项数据。')
        return false
      }
      this.price_data = this.selectedItems.map(res => {
        let {centerId, medicalItemId, medicalItemName, inPrice, dualPrice, salePrice, specifications, socialSecurityPrice, dialysisName, manufacturer, supplierName} = {...res}
        let params = {
          centerId,
          medicalId: medicalItemId,
          medicalName: medicalItemName,
          purchasingPrice: inPrice,
          upSalePrice: salePrice,
          salePrice: 0,
          dualPrice,
          specifications,
          manufacturer,
          supplierName,
          socialSecurityPrice,
          dialysisName,
        }
        return params
      })
      this.changePriceModal = true
    },
    changePriceNotify () {
      if (this.table_data.length === 0) {
        this.$Message.warning(`请先进行数据查询`)
        return false
      }
      if (this.notifyFlag) return false
      this.notifyFlag = true
      this.columns.unshift({
        type: 'selection',
        width: 20,
        align: 'center'
      })
      this.setChecked()
    },
    // cy 取消合并
    notifyCancel () {
      if (this.notifyFlag) {
        this.notifyFlag = false
        this.supplierInfo = {}
        // this.mergeModal = false
        this.$refs.statisticTable.selectAll(false)
        // 清空ids集合
        this.clearSelectedIds()
        this.columns.shift()
      }
      // this.getFormList()
    },
    dataTypeChange () {
      // this.clearSelectedIds()
      this.setData()
      setTimeout(() => {
        this.setChecked()
      }, 0)
    },
    // 前端数据筛选
    handleFilterData () {
      let flag = this.table_data.length > 0
      let data = []
      // 选中的报表类型tabCheckedId为5
      // 数据类型dataType 为异常
      if (this.tabCheckedId === 5 && this.dataType === 2) {
        data = flag && this.goodsSearch !== '' ? toFilterKey(
          this.table_warning_data,
          'medicalItemName,manufacturer',
          this.goodsSearch
        ) : this.table_warning_data
      // 数据类型为 低于采购价
      } else if (this.tabCheckedId === 5 && this.dataType === 3) {
        data = flag && this.goodsSearch !== '' ? toFilterKey(
          this.table_lowerSalePrice_data,
          'medicalItemName,manufacturer',
          this.goodsSearch
        ) : this.table_lowerSalePrice_data
      } else {
        data = flag && this.goodsSearch !== '' ? toFilterKey(
          this.table_data,
          'medicalItemName,manufacturer',
          this.goodsSearch
        ) : this.table_data
      }
      //
      if (this.medicalDataType === 2) {
        data = data.filter(res => {
          return res.hiCenterCode === null
        })
      }

      if ([5].includes(this.tabCheckedId) && (data.length !== 0 && data[data.length - 1].id !== null)) {
        data = this.countCXData(data)
      }
      this.table_filter_data = data
      return data
    },
    countCXData (data) {
      let total_qty = 0
      let total_price = 0
      data.forEach(res => {
        if (res.inQty > 0) total_qty = accAdd(total_qty, res.inQty)
        if (res.purchaseAmount > 0) total_price = accAdd(total_price, res.purchaseAmount)
      })
      this.cx_totalData = {
        id: null,
        medicalItemName: '合计',
        // inQty: Math.round(total_qty),
        // purchaseAmount: Math.round(total_price)
        inQty: total_qty,
        purchaseAmount: total_price.toFixed(2)
      }
      // console.log('countCXData', total_qty, total_price)
      data.push(this.cx_totalData)
      return data
    },
    changeKcPage (page) {
      let begin = (this.kcStartPage - 1) * this.kcPageSize
      let data = this.handleFilterData()
      this.table_show_data = data.slice(begin, begin + this.kcPageSize)
      setTimeout(() => {
        this.setChecked()
      }, 0)
    },
    setData () {
      if (this.table_data.length === 0) {
        this.$Message.warning(`请先进行数据查询`)
        return false
      }
      this.kcStartPage = 1
      let data = this.handleFilterData()
      this.dataCount = data.length
      this.table_show_data = data.slice(0, this.kcPageSize)
    },
    // cy 导出
    exportTable () {
      this.$refs.statisticTable.exportCsv({
        filename: `${this.subTitle}${this.goodsSearch?'(筛选:'+this.goodsSearch+')':''}`,
        columns: this.columns.slice(1),
        // noHeader: true,
        // data: this.medicalItemFilter
        data: this.table_filter_data.filter((item,index)=>{
          if(item.productionDate){
            item.productionDate = item.productionDate.substring(0,10);
          }
          if(item.qualityDate){
            item.qualityDate = item.qualityDate.substring(0,10)
          }
          return index<this.table_filter_data.length
        })
      })
    },
    // 获取机构
    getHospital (hospitalList) {
      this.hospitalList = hospitalList
      this.hospitalCheckedId = hospitalList[0].dialysisId
    },
    // 切换医院
    changeHospital (id, list) {
      id = id || 'abc3d60b474c4fef946a66887348c41a'
      this.hospitalCheckedId = id
      this.hospitalCheckedName = this.hospitalList.filter(
        v => v.dialysisId === id
      )[0].dialysisName
    },
    chooseArr1 (id, flag = false) {
      this.notifyCancel()
      this.clearSelectedIds()
      this.tabCheckedId = id
      this.goodsSearch = '' // 清空搜索
      let url = ''
      // this.columns = []
      // this.table_data = []
      this.dataCount = 0
      this.kcStartPage = 1
      this.columns = []
      this.table_data = []
      this.table_show_data = []
      this.table_warning_data = []
      this.table_lowerSalePrice_data = []
      let params = {}
      switch (id) {
        case 1:
          this.columns = this.column1
          this.subTitle = '用量统计表'
          url = 'MaterialsStatistica/MaterialsStatistica/UsageStatistics'
          params = {
            // medicalItemName: '',
            medicalItemType: this.selectedItemTypeIndex,
            centerId: this.hospitalCheckedId,
            beginTime: this.dateData[0],
            endTime: this.dateData[1],
            GroupType:this.GroupType,
          }
          break
        case 2:
          this.columns = this.column2
          this.subTitle = '低库存预警表'
          url = 'MaterialsStatistica/MaterialsStatistica/LowInventoryWarning'
          params = {
            centerId: this.hospitalCheckedId
          }
          break
        case 3:
          this.columns = this.column3
          this.subTitle = '高积压库存统计表'
          url = 'MaterialsStatistica/MaterialsStatistica/HighOverstockStatistics'
          params = {
            medicalItemType: this.selectedItemTypeIndex,
            centerId: this.hospitalCheckedId,
            medicalItemName: ''
          }
          break
        case 4:
          this.columns = this.column4
          this.subTitle = '供应商入库统计表'
          url = 'MaterialsStatistica/MaterialsStatistica/SuppliePutInStorage'
          params = {
            medicalItemType: this.selectedItemTypeIndex,
            centerId: this.hospitalCheckedId,
            beginTime: this.dateData[0],
            endTime: this.dateData[1],
            supplierId: this.selectedSupplierId || ''
          }
          break
        case 5:
          this.columns = this.column5
          this.subTitle = '库存统计表'
          url = 'MaterialsStatistica/MaterialsStatistica/GoodsInStockd'
          params = {
            // cy 后台不支持查询全部，且“0”有另外用途，故传空
            centerId:
              this.hospitalCheckedId === '0' ? '' : this.hospitalCheckedId,
            medicalItemType: this.selectedItemTypeIndex
            // MedicalItemName: ''
          }
          break
        case 6:
          this.columns = this.column6
          this.subTitle = '月均用量统计表'
          url = 'MaterialsStatistica/MaterialsStatistica/MonthGoodsInStockdModel'
          params = {
            medicalItemType: this.selectedItemTypeIndex,
            centerId: this.hospitalCheckedId,
            supplierId: this.selectedSupplierId || '',
            beginTime: this.dateData[0],
            endTime: this.dateData[1],
          }
          break
      }
      if (this.tabCheckedId === 5) {
        // this.columns.splice(5,0,{ title: '上月用量', key: 'monthAverage', align: 'center', minWidth: 80 })
        if (this.columns[2].key !== 'hiCenterCode' && [1, 2].includes(this.selectedItemTypeIndex)) {
        // 向库存统计表 类型为 药品，耗材的table中添加 医保编码列
          this.columns.splice(2, 0, { title: '医保编码', key: 'hiCenterCode', minWidth: 80 })
        } else if (this.columns[2].key === 'hiCenterCode' && ![1, 2].includes(this.selectedItemTypeIndex)) {
          this.columns.splice(2, 1)
        }
      }
      flag && this.loadTable(url, params)
    },
    loadTable (url, params) {
      this.table_loading = true
      this.table_data = []
      this.printDatas = Object.assign(this.printDatas, {
        api: url,
        params
      })
      this.swsApi
        .swsPostCouldCancel(url, params)
        .then(res => {
          if (res.data.success) {
            if (this.tabCheckedId === 5) {
              this.table_data = res.data.result.map((res, index) => {
                if (res.salePrice > res.socialSecurityPrice && (res.socialSecurityPrice !== 0 && res.socialSecurityPrice !== null)) {
                    res.dataType = 2
                  // if (res.inQty === 0) return
                  // this.table_warning_data.push(res)
                  // 这样写报错 _isHover of undefined
                  if (res.inQty !== 0) {
                    this.table_warning_data.push(res)
                  }
                  // 销售价低于采购价
                } else if (res.salePrice<res.inPrice){
                  res.dataType = 3
                  this.table_lowerSalePrice_data.push(res)
                } else {
                  res.dataType = 1
                }
                return res
              })
            } else {
              this.table_data = res.data.result
            }
            let data = this.handleFilterData()
            this.dataCount = data.length
            this.table_show_data = data.slice(0, this.kcPageSize)
          }
          this.table_loading = false
        })
        .catch(e => {
          this.table_loading = false
          console.log('请求出错，请稍后再试', e)
          this.$Message.error('请求出错，请稍后再试', e)
        })
    },
    chooseDate (value) {
      this.dateData = value
    },
    print () {
      let url = window.location.href.split('#')[0]
      // 缓存打印数据
      this.printDatas = Object.assign(this.printDatas, {
        columns: JSON.parse(JSON.stringify(this.columns)),
        title: this.subTitle
      })

      const { api, columns, title, params } = this.printDatas
      this.setTitle(title)
      this.setApi(api)
      this.setColumns(columns)
      this.setParams(params)

      window.open(`${url}#/print/material_statistics`)
    }
  }
}
</script>

<style lang="less" scoped="scoped">
#statistics {
  height: 100%;
  font-size: 14px;
  .content {
    .tab-content {
      height: 100%;
      overflow-y: auto;
      padding-top: 20px;
      background: #ffffff;
      .info {
        padding: 0 10px;
        padding-bottom: 10px;
        background: #ffffff;
        h2 {
          margin: 8px 0;
          text-align: center;
          font-weight: normal;
          font-size: 18px;
        }
      }
    }
    .tab-wrapper {
      height: 100%;
      // padding: 20px 15px;
      background: #ffffff;
      // position: relative;
      .btn-group {
        position: absolute;
        right: 40px;
        z-index: 10;
        /deep/ .ivu-btn-primary {
          background: #4f95e8;
          border-color: #4f95e8;
          box-shadow: 2px 2px 6px rgba(79, 149, 232, 0.35);
        }
      }
    }
    .tab-panel {
      height: 32px;
    }
    .out {
      float: right;
    }
    .notify-btn-goups {
      float: left;
      margin-top: 16px;
      .ivu-select {
        position: relative !important;
      }
    }
  }
}
</style>
