<template>
  <div class="energe_management">
    <!-- 新增记录 -->
    <Modal v-model="addModal" class="add_data_prefix" :mask-closable="false" width="800">
      <p slot="header" class="text-center">新增记录</p>
      <Form
        ref="formValidateRef"
        :model="formValidate"
        :rules="ruleValidate"
        :label-width="90"
        style="margin-right: 30px;"
      >
        <Row>
          <Col span="12">
            <FormItem label="机构" prop="centerId">
              <Select v-model="formValidate.centerId" placeholder="请选择机构">
                <Option
                  v-for="center in hospitalList"
                  v-if="center.dialysisId!='0'"
                  :key="center.dialysisId"
                  :value="center.dialysisId"
                >{{center.dialysisName}}</Option>
              </Select>
            </FormItem>
          </Col>
          <Col span="12">
            <FormItem label="能耗类型" prop="itemType">
              <Select v-model="formValidate.itemType" placeholder="请选择类型">
                <Option
                  v-for="type in energeList"
                  v-if="type.index!=0"
                  :key="type.index"
                  :value="type.index.toString()"
                >{{type.name}}</Option>
              </Select>
            </FormItem>
          </Col>
          <Col span="6">
            <FormItem label="起度" prop="startDosage">
              <InputNumber
                :min="0"
                v-model="formValidate.startDosage"
                @on-change="changeDosage"
                style="width:100%"
              ></InputNumber>
            </FormItem>
          </Col>
          <Col span="6">
            <FormItem label="止度" prop="endDosage">
              <InputNumber
                :max="999999"
                :min="1"
                v-model="formValidate.endDosage"
                @on-change="changeDosage"
                style="width:100%"
              ></InputNumber>
            </FormItem>
          </Col>
          <Col span="6">
            <FormItem label="量" prop="dosage">
              <InputNumber :min="0" v-model="formValidate.dosage" disabled style="width:100%"></InputNumber>
            </FormItem>
          </Col>
          <Col span="6">
            <FormItem label="金额" prop="amountPriec">
              <InputNumber :min="0" v-model="formValidate.amountPriec" style="width:100%"></InputNumber>
            </FormItem>
          </Col>
        </Row>
        <Row>
          <Col span="12">
            <FormItem label="缴费时间" prop="month">
              <DatePicker v-model="formValidate.month" placement="bottom-end" placeholder="请选择日期"></DatePicker>
            </FormItem>
          </Col>
          <Col span="24">
            <FormItem label="备注" prop="remarks">
              <Input v-model="formValidate.remarks" placeholder="请输入备注" type="textarea" :rows="3"/>
            </FormItem>
          </Col>
        </Row>
      </Form>
      <div slot="footer" style="text-align:center;">
        <Button type="primary" @click="handleAdd('formValidateRef')" :loading="handleBtn">新增</Button>
        <Button type="default" @click="addModal=false;handleReset('formValidateRef')">取消</Button>
      </div>
    </Modal>

    <!-- 删除物品 -->
    <Modal v-model="deleteModal" width="400" class-name="vertical-center-modal">
      <p slot="header">
        <span>删除确认</span>
      </p>
      <div style="text-align:center; padding: 40px 0; font-size: 16px;">
        <p>
          <Icon type="ios-help-circle" color="#ff81a3" size="30" style="margin-right: 20px;"/>删除后不可恢复，您确定删除吗？
        </p>
      </div>
      <div slot="footer" class="center">
        <Button type="primary" @click="handleDetailDel">确定</Button>
        <Button @click="deleteModal=false;">取消</Button>
      </div>
    </Modal>

    <!-- 新增营业费用 -->
    <Modal
      v-model="addDetailModal"
      class="add_data_prefix"
      @on-cancel="handleReset('detailValidate')"
      :mask-closable="false"
      width="620"
    >
      <p slot="header" class="text-center">{{isAdd ? '新增费用' : '修改费用'}}</p>
      <Form
        ref="detailValidate"
        :model="detailValidate"
        :rules="detailRuleValidate"
        :label-width="90"
        style="margin-right: 30px;"
      >
        <Row>
          <Col span="12">
            <FormItem label="机构" prop="centerId">
              <Select v-model="detailValidate.centerId" :disabled="!isAdd" placeholder="请选择机构">
                <Option
                  v-for="center in hospitalList"
                  v-if="center.dialysisId!='0'"
                  :key="center.dialysisId"
                  :value="center.dialysisId"
                >{{center.dialysisName}}</Option>
              </Select>
            </FormItem>
          </Col>
          <Col span="12">
            <FormItem label="月份" prop="month">
              <DatePicker
                :disabled="!isAdd"
                v-model="detailValidate.month"
                type="month"
                placement="bottom-end"
                placeholder="请选择月份"
              ></DatePicker>
            </FormItem>
          </Col>
          <Col span="12">
            <FormItem label="房租(门诊)" prop="mzbzj">
              <InputNumber :min="0" v-model="detailValidate.mzbzj" style="width:100%"></InputNumber>
            </FormItem>
          </Col>
          <Col span="12">
            <FormItem label="房租(宿舍)" prop="sszj">
              <InputNumber :min="0" v-model="detailValidate.sszj" style="width:100%"></InputNumber>
            </FormItem>
          </Col>
          <Col span="12">
            <FormItem label="物管" prop="wg">
              <InputNumber :min="0" v-model="detailValidate.wg" style="width:100%"></InputNumber>
            </FormItem>
          </Col>
          <Col span="12">
            <FormItem label="水电" prop="sd">
              <InputNumber :min="0" v-model="detailValidate.sd" style="width:100%"></InputNumber>
            </FormItem>
          </Col>
          <Col span="12">
            <FormItem label="工资" prop="gz">
              <InputNumber :min="0" v-model="detailValidate.gz" style="width:100%"></InputNumber>
            </FormItem>
          </Col>
          <Col span="12">
            <FormItem label="社保" prop="sb">
              <InputNumber :min="0" v-model="detailValidate.sb" style="width:100%"></InputNumber>
            </FormItem>
          </Col>
          <Col span="12">
            <FormItem label="福利" prop="fl">
              <InputNumber :min="0" v-model="detailValidate.fl" style="width:100%"></InputNumber>
            </FormItem>
          </Col>
          <Col span="12">
            <FormItem label="服务费" prop="axjj">
              <InputNumber :min="0" v-model="detailValidate.axjj" style="width:100%"></InputNumber>
            </FormItem>
          </Col>
          <Col span="12">
            <FormItem label="折旧费" prop="zjf">
              <InputNumber :min="0" v-model="detailValidate.zjf" style="width:100%"></InputNumber>
            </FormItem>
          </Col>
          <Col span="12">
            <FormItem label="装修" prop="zx">
              <InputNumber :min="0" v-model="detailValidate.zx" style="width:100%"></InputNumber>
            </FormItem>
          </Col>
          <Col span="12">
            <FormItem label="车辆" prop="clfy">
              <InputNumber :min="0" v-model="detailValidate.clfy" style="width:100%"></InputNumber>
            </FormItem>
          </Col>
          <Col span="12">
            <FormItem label="其他" prop="qt">
              <InputNumber :min="0" v-model="detailValidate.qt" style="width:100%"></InputNumber>
            </FormItem>
          </Col>
          <Col span="24">
            <FormItem label="备注">
              <Input
                v-model="detailValidate.remarks"
                placeholder="请输入备注"
                type="textarea"
                :rows="3"
              />
            </FormItem>
          </Col>
        </Row>
      </Form>
      <div slot="footer" style="text-align: center">
        <Button type="primary" @click="addAndSave">保存</Button>
        <Button type="default" @click="handleReset('detailValidate')">取消</Button>
      </div>
    </Modal>

    <financial-template
      :tabData="tabData"
      @on-change="changeTab"
      @on-initial-hospital="getHospital"
    >
      <div slot="content" class="tab-wrapper">
        <div class="tab-content">
          <div class="table">
            <div class="table-operate">
              <div class="button-group">
                <span>
                  机构选择：
                  <Select
                    v-model="hospitalCheckedId"
                    filterable
                    placeholder
                    @on-change="changeHospital"
                    style="width: 160px;margin-right:10px;"
                  >
                    <Option
                      v-for="item in hospitalList"
                      :key="item.dialysisId"
                      :value="item.dialysisId"
                    >{{item.dialysisName}}</Option>
                  </Select>
                </span>
                <span v-show="tabCheckedInfo.tabCheckedId === 2">
                  能源类别：
                  <Select
                    style="width:120px;margin-right:10px;"
                    v-model="selectedType"
                    placeholder="请选择"
                    @on-change="changeType"
                  >
                    <Option
                      :value="item.index"
                      v-for="item in selectList"
                      :key="item.index"
                    >{{item.name}}</Option>
                  </Select>
                </span>
                <span>
                  日期：
                  <DatePicker
                    :value="formDate"
                    @on-change="chooseDate"
                    type="daterange"
                    :options="options"
                    placeholder="请选择时间段"
                  ></DatePicker>
                </span>
                <!-- <Input
                  v-model="searchKey"
                  search
                  enter-button
                  @on-search="keySearch"
                  placeholder="请输入关键词..."
                  style="width: 200px;margin-left:10px"
                />-->
              </div>
              <div class="operate-right">
                <Button
                  type="success"
                  @click="addDetailModal = true; isAdd = true"
                  v-show="tabCheckedInfo.tabCheckedId === 1"
                  v-permission="buttonRole.NHGL_TJ"
                >添加</Button>
                <!-- <Button type="primary" v-permission="buttonRole.NHGL_DC" @click="exportExl">导出</Button> -->
                <!-- <Button @click="printData">打印</Button> -->
              </div>
            </div>
            <h2
              style="text-align:center;margin:10px 0;"
            >{{hospitalCheckedId==0?'全部机构':hospitalCheckedName}}-{{tabCheckedInfo.tabCheckedId === 2 ? selectedTypeName : ''}}{{tabCheckedInfo.tabTitle}}</h2>

            <div class v-show="tabCheckedInfo.tabCheckedId === 2">
              <Table
                :data="table_data"
                ref="table"
                :columns="table_columns"
                :height="tableHeight"
                :loading="tableLoading"
              >
                <template slot-scope="{ row, index }" slot="startDosage">
                  <InputNumber
                    :min="0"
                    v-model="editStartDosage"
                    v-if="editIndex === index"
                    @on-change="changeDosage"
                  ></InputNumber>
                  <span v-else>{{ row.startDosage }}</span>
                </template>

                <template slot-scope="{ row, index }" slot="endDosage">
                  <InputNumber
                    :min="0"
                    v-model="editEndDosage"
                    v-if="editIndex === index"
                    @on-change="changeDosage"
                  ></InputNumber>
                  <span v-else>{{ row.endDosage }}</span>
                </template>

                <template slot-scope="{ row, index }" slot="dosage">
                  <InputNumber :min="0" v-model="editDosage" v-if="editIndex === index" disabled></InputNumber>
                  <span v-else>{{ row.dosage }}</span>
                </template>

                <template slot-scope="{ row, index }" slot="amountPriec">
                  <InputNumber :min="0" v-model="editAmountPriec" v-if="editIndex === index"></InputNumber>
                  <span v-else>{{ row.amountPriec }}</span>
                </template>

                <template slot-scope="{ row, index }" slot="month">
                  <DatePicker
                    placeholder="请选择日期"
                    type="date"
                    v-model="editMonth"
                    v-if="editIndex === index"
                    style="width: 102px"
                  ></DatePicker>
                  <span v-else>{{ new Date(row.month).toLocaleDateString().replace(/\//g, '-') }}</span>
                </template>

                <template slot-scope="{ row, index }" slot="remarks">
                  <Input v-model="editRemarks" v-if="editIndex === index"></Input>
                  <span v-else>{{ row.remarks }}</span>
                </template>

                <template slot-scope="{ row, index }" slot="action">
                  <div v-if="editIndex === index" class="button-group">
                    <Button
                      type="primary"
                      style="margin-right: 10px;"
                      size="small"
                      @click="handleSave(row, index)"
                    >保存</Button>
                    <Button size="small" @click="editIndex = -1">取消</Button>
                  </div>
                  <div v-else>
                    <Tooltip content="修改" v-permission="buttonRole.NHGL_XG" placement="top">
                      <Icon
                        type="md-create"
                        size="22"
                        color="#4f95e8"
                        @click="handleEdit(row, index)"
                        style="cursor: pointer;font-size: 18px;"
                      ></Icon>
                    </Tooltip>
                    <Tooltip content="删除" v-permission="buttonRole.NHGL_SC" placement="top">
                      <Icon
                        type="md-close"
                        size="22"
                        color="red"
                        @click="delBtn(row, index)"
                        style="cursor: pointer"
                      ></Icon>
                    </Tooltip>
                  </div>
                </template>
              </Table>
              <div class="pagination" v-if="dataCount > pageSize">
                <Page
                  :total="dataCount"
                  :page-size="pageSize"
                  :current.sync="currentPage"
                  @on-change="handleChangePage"
                />
              </div>
            </div>

            <div class v-show="tabCheckedInfo.tabCheckedId === 1">
              <Table
                ref="table"
                border
                class="default-table"
                :data="detailTableData"
                :loading="tableLoading"
                :columns="detailColumns"
                :height="tableHeight"
              ></Table>
              <div class="pagination">
                <Page
                  class="page"
                  :total="detailDataCount"
                  show-total
                  :current="detailCurrent"
                  :page-size="detailPageSize"
                  @on-change="changePage"
                ></Page>
              </div>
            </div>
          </div>
          <!-- <div >
            <h2 class="title">营业费用明细</h2>
            <div class="add">
              <Button type="success" @click="addDetailModal=true">新增</Button>
            </div>
          </div>-->
        </div>
      </div>
    </financial-template>
  </div>
</template>

<script>
import financialTemplate from '@/components/financial-template'
import Operate from '@/components/operate'
import { setTimeCurrent } from '@/libs/util'
const BUTTONROLE = {
  NHGL_DC: 'NHGL_DC',
  NHGL_TJ: 'NHGL_TJ',
  NHGL_XG: 'NHGL_XG',
  NHGL_SC: 'NHGL_SC'
}

export default {
  name: 'energe_management',
  components: {
    financialTemplate,
    Operate
  },
  data () {
    return {
      tableHeight: 0,
      handleBtn: false,
      addModal: false,
      formValidate: {
        centerId: '',
        itemType: '',
        month: '',
        dosage: 0,
        amountPriec: 0,
        startDosage: 0,
        endDosage: 1,
        remarks: ''
      },
      ruleValidate: {
        centerId: [
          { required: true, message: '机构不能为空', trigger: 'blur' },
          { message: '机构不能为空', trigger: 'change' }
        ],
        itemType: [
          { required: true, message: '类型不能为空', trigger: 'blur' },
          { message: '类型不能为空', trigger: 'change' }
        ],
        month: [
          {
            required: true,
            type: 'date',
            message: '日期不能为空',
            trigger: 'change'
          }
        ],
        dosage: [
          {
            validator: (rule, value, callback) => {
              if (value >= 0) {
                callback()
              } else {
                callback(new Error('量不能为负值'))
              }
            },
            // required: true,
            // type: 'number',
            // message: '量不能为空',
            trigger: 'change'
          }
        ],
        amountPriec: [
          {
            required: true,
            type: 'number',
            message: '金额不能为空',
            trigger: 'blur'
          }
        ],
        startDosage: [
          {
            required: true,
            type: 'number',
            message: '起度不能为空',
            trigger: 'blur'
          }
        ],
        endDosage: [
          {
            required: true,
            type: 'number',
            message: '止度不能为空',
            trigger: 'blur'
          }
        ]
      },
      deleteModal: false,
      delDatas: {
        delName: '',
        delId: ''
      },

      editIndex: -1,
      editRemarks: '',
      editAmountPriec: 0,
      editDosage: 0,
      editEndDosage: 0,
      editStartDosage: 0,
      editMonth: '',

      searchKey: '',
      selectedType: 0,
      // selectedTypeName: '',
      selectList: [],
      energeList: [
        { index: 0, name: '全部' },
        { index: 1, name: '水' },
        { index: 2, name: '电' },
        { index: 3, name: '气' },
        { index: 4, name: '其他' }
      ],
      statisticList: [
        { index: 1, name: '按年度' },
        { index: 2, name: '按月度' }
      ],
      formDate: '',
      options: {
        disabledDate (date) {
          return date && date.valueOf() > Date.now()
        }
      },
      currentPage: 1, // 当前页
      pageSize: 10,
      dataCount: 0,
      hospitalCheckedId: 0, // 选中的透析中心id
      hospitalCheckedName: '', // 选中的透析中心name
      hospitalList: [], // 透析中心列表
      tabCheckedInfo: {
        tabTitle: '',
        tabCheckedId: 1 // 选中的标签类型id
      },
      tabData: [
        { childKey: 1, childValue: '营业费用' }
        // { childKey: 2, childValue: '能耗统计' }
      ],
      table_data: [],
      table_columns: [],
      table_statistic_base: [
        { title: '机构名称', key: 'centerName' },
        { title: '用水量', key: 'waterSum' },
        { title: '水费(元)', key: 'waterPrice' },
        { title: '用电量', key: 'fuelSum' },
        { title: '电费(元)', key: 'fuelPrice' }
      ],
      table_manage_base: [
        { title: '机构名称', key: 'centerName' },
        { title: '起度', key: 'startDosage', slot: 'startDosage' },
        { title: '止度', key: 'endDosage', slot: 'endDosage' },
        { title: '量', key: 'dosage', slot: 'dosage' },
        { title: '金额(元)', key: 'amountPriec', slot: 'amountPriec' },
        { title: '缴费时间', key: 'month', slot: 'month' },
        { title: '备注', key: 'remarks', slot: 'remarks' }
      ],
      table_operate: [
        {
          title: '操作',
          align: 'center',
          width: 130,
          fixed: 'right',
          render: (h, params) => {
            return (
              <Operate
                showWatch={false}
                handleDelete={_ => {
                  this.detailId = params.row.id
                  this.deleteModal = true
                }}
                handleEdit={_ => {
                  this.isAdd = false
                  this.detailId = params.row.id
                  this.addDetailModal = true
                  for (let key in this.detailValidate) {
                    this.detailValidate[key] = params.row[key]
                  }
                }}
                permissionDelete={this.buttonRole.NHGL_SC}
                permissionEdit={this.buttonRole.NHGL_XG}
              />
            )
          }
        }
      ],
      url: '',
      tableLoading: false,
      buttonRole: BUTTONROLE,

      // 营业费用记录
      addDetailModal: false,
      detailRuleValidate: {
        centerId: [
          { required: true, message: '机构不能为空', trigger: 'blur' },
          { message: '机构不能为空', trigger: 'change' }
        ],
        month: [
          {
            required: true,
            type: 'date',
            message: '日期不能为空',
            trigger: 'blur'
          }
        ],
        mzbzj: [
          {
            required: true,
            type: 'number',
            message: '金额不能为空',
            trigger: 'blur'
          }
        ],
        sszj: [
          {
            required: true,
            type: 'number',
            message: '金额不能为空',
            trigger: 'blur'
          }
        ],
        wg: [
          {
            required: true,
            type: 'number',
            message: '金额不能为空',
            trigger: 'blur'
          }
        ],
        sd: [
          {
            required: true,
            type: 'number',
            message: '金额不能为空',
            trigger: 'blur'
          }
        ],
        gz: [
          {
            required: true,
            type: 'number',
            message: '金额不能为空',
            trigger: 'blur'
          }
        ],
        sb: [
          {
            required: true,
            type: 'number',
            message: '金额不能为空',
            trigger: 'blur'
          }
        ],
        fl: [
          {
            required: true,
            type: 'number',
            message: '金额不能为空',
            trigger: 'blur'
          }
        ],
        axjj: [
          {
            required: true,
            type: 'number',
            message: '金额不能为空',
            trigger: 'blur'
          }
        ],
        zjf: [
          {
            required: true,
            type: 'number',
            message: '金额不能为空',
            trigger: 'blur'
          }
        ],
        zx: [
          {
            required: true,
            type: 'number',
            message: '金额不能为空',
            trigger: 'blur'
          }
        ],
        clfy: [
          {
            required: true,
            type: 'number',
            message: '金额不能为空',
            trigger: 'blur'
          }
        ],
        qt: [
          {
            required: true,
            type: 'number',
            message: '金额不能为空',
            trigger: 'blur'
          }
        ]
      },
      detailValidate: {
        centerId: -1,
        month: '',
        mzbzj: 0,
        sszj: 0,
        wg: 0,
        sd: 0,
        gz: 0,
        sb: 0,
        fl: 0,
        axjj: 0,
        zjf: 0,
        zx: 0,
        clfy: 0,
        qt: 0,
        remarks: ''
      },

      detailDataCount: 0,
      detailPageSize: 10,
      detailCurrent: 1,
      detailTableData: [],
      detailDefaultColumns: [
        // 营业费用表
        { title: '机构名称', key: 'centerName', width: 140 },
        {
          title: '时间',
          key: 'month',
          align: 'center',
          width: 120,
          render: (h, params) => {
            let date = new Date(params.row.month).toLocaleDateString()
            return <span>{date}</span>
          }
        },
        {
          title: '租金',
          align: 'center',
          children: [
            { title: '门诊部', key: 'mzbzj', align: 'center', width: 70 },
            {
              title: '宿舍',
              key: 'sszj',
              align: 'center',
              width: 70
            }
          ]
        },
        { title: '物管', key: 'wg', align: 'center', width: 100 },
        { title: '水电', key: 'sd', align: 'center', width: 100 },
        { title: '工资', key: 'gz', align: 'center', width: 80 },
        { title: '社保、公积金', key: 'sb', align: 'center', width: 100 },
        { title: '福利费', key: 'fl', align: 'center', width: 120 },
        { title: '服务费', key: 'axjj', align: 'center', width: 120 },
        { title: '折旧费', key: 'zjf', align: 'center', width: 120 },
        { title: '装修/无形资产摊销', key: 'zx', align: 'center', width: 160 },
        { title: '车辆费用', key: 'clfy', align: 'center', width: 120 },
        { title: '其他', key: 'qt', align: 'center', width: 120 },
        { title: '小计', key: 'totalAmountPriec', align: 'center', width: 120 }
      ],
      detailColumns: [],
      detailId: '',
      isAdd: false
    }
  },
  computed: {
    selectedTypeName: {
      // return this.selectList.filter(item => item.index == this.selectedType)[0].name
      get () {
        // console.log(this.selectedType)
        // return this.selectedType
        let filter = this.selectList.filter(
          item => item.index === this.selectedType
        )
        return filter.length ? filter[0].name : ''
      },
      set (val) {
        console.log(val)
      }
    },
    itemDetails () {
      let obj = []
      for (let key in this.detailValidate) {
        if (!['centerId', 'month', 'remarks'].includes(key)) {
          obj.push({
            itemType: key,
            amountPriec: this.detailValidate[key]
          })
        }
      }

      return obj
    }
  },
  created () {
    this.changeTab()
  },
  mounted () {
    // cy 调整table高度
    this.tableHeight = document.documentElement.clientHeight - 338
  },
  methods: {
    // 新增保存费用明细
    addAndSave () {
      this.$refs.detailValidate.validate(vaild => {
        if (vaild) {
          let { centerId, remarks, month } = this.detailValidate
          let params = {
            centerId,
            remarks,
            month,
            itemDetails: this.itemDetails
          }
          if (this.isAdd) {
            params.month = setTimeCurrent(params.month)
          } else {
            params.id = this.detailId
          }
          this.swsApi
            .swsPost('WaterFuel/AddUpdateWaterFuel', params)
            .then(res => {
              if (res.data.success) {
                this.$Message.success('保存成功！')
                this.addDetailModal = false
                this.handleReset('detailValidate')
                this.setDatas(this.tabCheckedInfo.tabCheckedId)
              }
            })
            .catch(e => {})
        }
      })
    },
    // cy 点击删除按钮
    delBtn (row, index) {
      this.deleteModal = true
      this.delDatas.delId = row.id
    },
    // cy 删除物品
    handleDel () {
      this.handleBtn = true
      this.swsApi
        .swsGet(`WaterFuel/DelWaterFuel/${this.delDatas.delId}`)
        .then(res => {
          if (res.data.success) {
            this.deleteModal = false
            this.$Message.success(`删除成功！`)
            this.setDatas()
          } else {
            this.$Message.error(`删除操作失败，请稍后再试！`)
          }
          this.handleBtn = false
        })
        .catch(e => {
          this.$Message.error(`删除请求失败，请稍后再试！`)
        })
    },
    // fh 删除物品
    handleDetailDel () {
      this.deleteModal = false
      this.swsApi
        .swsGet(`WaterFuel/DelWaterFuel/${this.detailId}`)
        .then(res => {
          if (res.data.success) {
            this.$Message.success(`删除成功！`)
            this.setDatas()
          } else {
            this.$Message.error(`删除操作失败，请稍后再试！`)
          }
          this.handleBtn = false
        })
        .catch(e => {
          this.$Message.error(`删除请求失败，请稍后再试！`)
        })
    },
    handleReset (name) {
      this.$refs[name].resetFields()
      this.addDetailModal = false
    },
    addBtn () {
      this.addModal = true
    },
    handleAdd (name) {
      let params = { ...this.formValidate }
      // console.log(params)
      this.$refs[name].validate(valid => {
        if (valid) {
          this.handleBtn = true
          this.swsApi
            .swsPost('WaterFuel/AddUpdateWaterFuel', params)
            .then(res => {
              if (res.data.success) {
                // this.editIndex = -1
                this.addModal = false
                this.handleBtn = false
                // console.log(res.data.result)
                this.$Notice.success({
                  title: '添加记录成功'
                })
                this.setDatas()
              }
            })
            .catch(e => {
              this.handleBtn = false
              this.$Notice.error({
                title: '网络错误，请稍后再试',
                desc: e
              })
            })
        }
      })
    },
    handleSave (row, index) {
      let params = {
        id: row.id,
        // centerId: row.centerId,
        // itemType: row.itemType,
        month: this.editMonth,
        dosage: this.editDosage,
        amountPriec: this.editAmountPriec,
        startDosage: this.editStartDosage,
        endDosage: this.editEndDosage,
        remarks: this.editRemarks
      }
      // console.log(params)
      this.swsApi
        .swsPost('WaterFuel/AddUpdateWaterFuel', params)
        .then(res => {
          if (res.data.success) {
            this.editIndex = -1
            // console.log(res.data.result)
            this.setDatas()
          }
        })
        .catch(e => {
          this.$Notice.error({
            title: '网络错误，请稍后再试',
            desc: e
          })
        })
    },
    handleEdit (row, index) {
      this.editIndex = index
      this.editRemarks = row.remarks
      this.editAmountPriec = row.amountPriec
      this.editDosage = row.dosage
      this.editEndDosage = row.endDosage
      this.editStartDosage = row.startDosage
      this.editMonth = row.month
      // console.log(row)
    },
    exportExl () {
      this.$refs.table.exportCsv({
        filename: `${
          this.hospitalCheckedId === '0' ? '全部机构' : this.hospitalCheckedName
        }-${this.selectedTypeName}${this.tabCheckedInfo.tabTitle}`
      })
    },
    printData () {},
    keySearch (v) {
      this.setDatas()
      // this.searchKey = v
    },
    handleChangePage (i) {
      this.currentPage = i
      this.setDatas()
    },
    chooseDate (v) {
      this.formDate = v
      this.beginTime = v[0]
      this.endTime = v[1]
      this.setDatas()
    },
    // 切换医院
    changeHospital (id, list) {
      this.hospitalCheckedId = id
      this.hospitalCheckedName = this.hospitalList.filter(
        v => v.dialysisId === id
      )[0].dialysisName
      // this.changeTab()
      this.setDatas()
    },
    changeDosage () {
      if (this.addModal) {
        this.formValidate.dosage =
          this.formValidate.endDosage - this.formValidate.startDosage
      } else {
        this.editDosage = this.editEndDosage - this.editStartDosage
      }
    },
    // 选择表格类型
    changeTab (index) {
      index = index || 1
      this.tabCheckedInfo.tabCheckedId = index
      this.currentPage = 1
      // this.table_data = []
      // if (index <= 2) {
      this.setBase(index)
      // }
    },
    setBase (index) {
      index = index || 1
      this.table_data = []
      switch (index) {
        case 1:
          // eslint-disable-next-line one-var
          let hasEdit = this._filterButton(this.buttonRole.NHGL_XG),
            hasDelete = this._filterButton(this.buttonRole.NHGL_SC)
          this.url = 'WaterFuel/WaterFuelList'
          this.selectList = this.energeList
          this.selectedType = 0
          if (hasEdit && hasDelete) {
            this.detailColumns = [
              ...this.detailDefaultColumns,
              ...this.table_operate
            ]
          } else {
            this.detailColumns = this.detailDefaultColumns
          }
          break
        case 2:
          this.url = 'WaterFuel/WaterFuelStatis'
          this.formDate = ''
          this.beginTime = ''
          this.endTime = ''
          this.selectedType = 1
          this.selectList = this.statisticList
          this.table_columns = this.table_statistic_base
          break
        default:
          break
      }
      this.selectedTypeName = this.selectList.filter(
        item => item.index === this.selectedType
      )[0].name
      this.setDatas()
    },
    changeType (index) {
      this.selectedType = index
      // this.selectedTypeName = this.selectList.filter(item => item.index == index)[0].name
      this.setDatas()
    },
    setDatas (index) {
      index = index || this.tabCheckedInfo.tabCheckedId
      this.tabCheckedInfo.tabTitle = this.tabData.filter(
        item => item.childKey === this.tabCheckedInfo.tabCheckedId
      )[0].childValue
      let params = {
        itemType: this.selectedType,
        centerId: this.hospitalCheckedId,
        beginTime: this.beginTime,
        endTime: this.endTime,
        // keywordValue: this.searchKey,
        pageNum: this.currentPage,
        pageSize: this.pageSize
      }
      this.tableLoading = true
      this.swsApi
        .swsPost(this.url, params)
        .then(res => {
          if (res.data.success) {
            if (this.tabCheckedInfo.tabCheckedId === 1) {
              this.detailTableData = res.data.result
              this.detailDataCount = res.data.dataCount
            }
            if (this.tabCheckedInfo.tabCheckedId === 2) {
              this.table_data = res.data.result
              this.dataCount = res.data.dataCount
            }
          }
          this.tableLoading = false
        })
        .catch(e => {
          console.log(e)
          this.tableLoading = false
        })
    },
    // 获取机构
    getHospital (hospitalList) {
      this.hospitalList = hospitalList
      this.hospitalCheckedId = hospitalList[0].dialysisId
    },
    changePage () {}
  }
}
</script>

<style scope="scoped" lang="less">
.energe_management {
  height: 100%;
  .content {
    background: #ffffff;
    .tab-wrapper {
      display: flex;
      flex-direction: column;
      height: 100%;
      .tab-panel {
        height: 32px;
      }
      .tab-content {
        flex: 1;
        overflow-y: auto;
        padding-top: 10px;
        .table {
          padding: 5px 10px;
          background: #ffffff;
          .table-operate {
            padding: 10px 0;
            display: flex;
            justify-content: space-between;
            .operate-right {
              margin-right: 20px;
            }
            .button-group {
              display: flex;
              justify-content: space-around;
              .search_box {
                margin: 0 5px;
              }
            }
          }
        }
        .energy_detail {
          padding: 0 10px;
          .title {
            text-align: center;
            margin-top: 10px;
          }
          .add {
            text-align: right;
          }
        }
      }
    }
  }
}
.add_data_prefix {
  /deep/ .ivu-input-prefix i {
    font-size: 12px;
  }
}
</style>
