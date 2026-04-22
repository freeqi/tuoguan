<template>
  <transition name="fade_enter">
    <div class="purchase_detail" v-show="purchase_detail">
      <Spin size="large" fix v-if="mergeLoading"></Spin>
      <div class="top">
        <div class="step-wrapper" ref="step">
          <Steps :current="currentStep" :status="currentStatus" v-if="!isMerge">
            <Step
              v-for="(item, index) in approvalProcessOutPuts"
              :key="index"
              :title="item.title"
              :content="`${item.content}${item.time ? '，时间：': ''}${item.time ? item.time : ''}`"
            ></Step>
          </Steps>
        </div>
        <p class="record">
          <span style="margin-left:0;" v-if="!isMerge">
            采购申请单号{{priceDetail.purchaseNo}}
            <template v-for="item in stateList">
              <span :class="item.color" v-if="item.id === priceDetail.groupAuditStatus" :key="item.id">{{item.label}}</span>
            </template>
          </span>
          <span style="margin-left:0;line-height: 33px;" v-if="isMerge">
            合并采购单
          </span>
          <span class="right button-group">
            <Button
              v-show="(priceDetail.groupAuditStatus === '2')&&!isLowValueType"
              v-permission="buttonRole.CGSQD_XZWP"
              type="success"
              @click="loadGoods"
            >新增物品</Button>
            <Button
              v-show="(priceDetail.groupAuditStatus === '2' && priceDetail.isGroupAdd === true &&!isLowValueType)"
              v-permission="buttonRole.CGSQD_XZWP"
              type="error"
              @click="detachFlag=true;">拆单</Button>
              <!-- //v-permission="buttonRole.CGSXD_SHENPI" -->
            <Button
              type="default"
              class="approval"
              v-show="['2', '7'].includes(priceDetail.groupAuditStatus)"
              v-permission="buttonRole.CGSQD_SHENPI"
              @click="showApproval"
            >审批</Button>
            <Button
              type="info"
              @click="multiSelectFlag = true;setChecked();"
              v-show="isShowCreatBtn"
              v-permission="buttonRole.CGSQD_SCDD"
            >生成订单</Button>
            <Button type="primary" v-permission="buttonRole.CGSQD_DC" @click="exportDetailTable">导出</Button>
            <Button type="info" v-permission="buttonRole.CGSQD_DYSQD" @click="printDetailTable">打印申请单</Button>
            <Button type="default" size="large" @click="hide" icon="md-undo">返回</Button>
          </span>
        </p>
        <p class="items-box">
          <span class="field">物品种类：</span>
          <span>{{priceDetail.itemType || 0 }}</span>
          <span class="field">合计总数量：</span>
          <span>{{priceDetail.totalQty || 0}}件</span>
          <span class="field">采购总金额：</span>
          <span class="red">￥{{priceDetail.actualPrice || 0}}</span>
          <span class="field">销售总金额：</span>
          <span class="red">￥{{priceDetail.salesTotalPrice || 0}}</span>
            <span class="field">物资类别：{{priceDetail.catalogueName || selectedItemName}}</span>
        </p>
      </div>
      <Divider class="split-line"></Divider>
      <div class="detail_table" ref="detail_table">
        <Table
          :height="tableHeight"
          :columns="tableColumns"
          :data="table_data"
          :loading="loading"
          :row-class-name="setRejectRowColor"
          @on-select-all="handleSelect"
          @on-select-all-cancel="handleSelect"
          @on-select="handleSelect"
          @on-select-cancel="handleSelect"
          ref="detailTable" highlight-row>
          <template slot-scope="{ row, index }" slot="approvalQty">
            <InputNumber :min="0" v-model="editApprovalQty" v-if="editIndex === index"></InputNumber>
            <span v-else>{{ row.approvalQty }}</span>
          </template>
          <!-- 固定资产的单价预算 -->
          <template slot-scope="{ row, index }" slot="advicePrice">
            <InputNumber :min="0" v-model="editAdvicePrice" v-if="editIndex === index"></InputNumber>
            <span v-else>{{ row.advicePrice }}</span>
          </template>
          <!-- 固定资产的申请理由 -->
          <template slot-scope="{ row, index }" slot="appReason">
            <Input
              type="textarea" :rows="3"
              v-model="editApplyReason" placeholder="请输入申请理由"
              v-if="editIndex === index"
            />
            <span v-else>{{ row.appReason }}</span>
          </template>

          <template slot-scope="{ row, index }" slot="inPrice">
            <InputNumber :min="0" v-model="editInPrice" v-if="editIndex === index" @on-change="inPriceChange"></InputNumber>
            <span v-else>{{ row.inPrice }}</span>
          </template>

          <template slot-scope="{ row, index }" slot="salePrice">
            <InputNumber :min="0" v-model="editSalePrice" v-if="editIndex === index"></InputNumber>
            <span v-else :class="{'high_light': (row.socialSecurityPrice!=0 && row.salePrice > row.socialSecurityPrice)}"
            >{{ row.salePrice }}</span>
          </template>

          <!-- 双控价 -->
          <!-- <template slot-scope="{ row, index }" slot="dualPrice">
            <Input
              v-model="editDualPrice"
              disabled
              v-if="editIndex === index && row.medicalItemRecordOutPut.medicalItemType==1"
            ></Input>
            <span v-else>{{ row.dualPrice }}</span>
          </template> -->

          <template slot-scope="{ row, index }" slot="supplierName">
            <Select transfer
              v-model="editSupplier"
              placeholder="请选择供应商"
              filterable
              v-if="editIndex === index"
            >
              <Option :value="item.id" v-for="item in supplierList" :key="item.id">{{item.name}}</Option>
            </Select>
            <span v-else>{{ row.supplierName}}</span>
          </template>

          <template slot-scope="{ row, index }" slot="remark">
            <Input
              type="textarea" :rows="3"
              v-model="editRemark" placeholder="请输入备注"
              v-if="editIndex === index"
            />
            <div v-else>
              <span v-if="row.remark===''"></span>
              <Tooltip :content="row.remark" placement="left" transfer max-width="200" v-else>
                <span class="ivu-table-cell-tooltip-content" style="width: 120px;">{{ row.remark}}</span>
              </Tooltip>
            </div>
          </template>

          <template slot-scope="{ row, index }" slot="action">
            <div v-if="editIndex === index" class="button-group">
              <Button type="primary" size="small" @click="handleSave(row, index)">保存</Button>
              <Button size="small" @click="editIndex = -1">取消</Button>
            </div>
            <!-- cy  <div v-else v-show="row.isUpdate"> -->
            <div v-else-if="editIndex !== index && row.dataState === 1">
              <Tooltip content="修改" placement="top" transfer v-permission="buttonRole.CGSQD_XG">
                <Icon
                  type="md-create"
                  size="22"
                  color="#4f95e8"
                  @click="handleEdit(row, index)"
                  style="cursor: pointer;font-size: 18px;"
                ></Icon>
              </Tooltip>
              <Tooltip content="拒绝" placement="top" transfer v-show="!isLowValueType" v-permission="buttonRole.CGSQD_SC">
                <Icon
                  type="md-remove-circle"
                  size="19"
                  color="red"
                  @click="delBtn(row, index)"
                  style="cursor: pointer"
                ></Icon>
              </Tooltip>
              <Tooltip content="拆分" placement="top" transfer v-show="!isLowValueType" v-permission="buttonRole.CGSQD_XG">
                <Icon
                  type="md-build"
                  size="19"
                  color="#2d8cf0"
                  @click="cutBtn(row, index)"
                  style="cursor: pointer"
                ></Icon>
              </Tooltip>
            </div>
            <div v-else-if="editIndex !== index && row.dataState === 2">
              <Tooltip content="恢复(未做)" transfer placement="top">
                <Icon
                  type="md-refresh"
                  size="20"
                  color="#fc4b4b"
                  style="cursor: pointer;"
                ></Icon>
              </Tooltip>
            </div>
          </template>
        </Table>
        <div class="merge-btn-goups" v-show="multiSelectFlag">
          <Button type="primary" style="margin-right:10px;" @click="creatRecordBtn">确定</Button>
          <Button style="margin-left:10px;" type="default" @click="multiSelectFlag=false;">取消</Button>
          <span style="margin-left:10px;">已经选中：{{multiSelections.length}} 项</span>
          <!-- <span style="margin-left:20px;color: red;">提示：合并采购单只能勾选未审批状态的采购单</span> -->
        </div>
        <div class="tips">
          <span>tips：表单行颜色为<strong style="color:green;">绿色</strong>代表集团端新增物品明细，<strong style="color:grey">灰色</strong>代表被禁用。</span>
        </div>
      </div>

      <!-- 审核 -->
      <Modal v-model="approvalModal" className="vertical-center-modal" width="400">
        <p slot="header" align="center">审核申请单</p>
        <Form :label-width="60" style="margin-bottom: 10px;">
          <FormItem label="审核状态">
            <Select v-model="approval.situation" style="width: 160px" placeholder="请选择是否同意">
              <Option :value="item.id" v-for="item in situationList.filter(res => {
                  if(priceDetail.groupAuditStatus==='7') return res.id !== '7'
                  else {return true}
                })" :key="item.id">{{item.name}}</Option>
            </Select>
          </FormItem>
          <FormItem label="审核意见" style="margin-bottom: 10px">
            <Input type="textarea" :rows="4" placeholder="请输入审核意见..." v-model="approval.advice"/>
          </FormItem>
          <!-- <p v-show="emptyPrice" class="emptyPrice">
            <span>存在销售价为0的物品</span>
          </p> -->
        </Form>
        <div slot="footer" class="center">
          <Button type="primary" v-show="!saveLoading" @click="saveApproval">审核</Button>
          <Button type="primary" v-show="saveLoading" :loading="true">审核中</Button>
          <Button type="default" @click="approvalModal = false">取消</Button>
        </div>
      </Modal>

      <!-- 新增物品 -->
      <Modal v-model="goodsFlag" class="goods_data" :mask-closable="false" width="1000">
        <p slot="header" class="text-center">新增物品</p>
        <Form
          ref="formValidateRef"
          :model="formValidate"
          :rules="ruleValidate"
          :label-width="90"
          style="margin-right: 30px;">
          <FormItem label="物品" prop="itemName">
            <Input v-model="formValidate.itemName" placeholder="请选择" readonly="readonly"/>
          </FormItem>
          <div
            style="width: 83%;height: 32px;position: absolute;top: 67px;left: 120px;"
            @click="loadGoods"
          ></div>
          <Row>
            <Col span="6">
              <FormItem label="数量" prop="inQty">
                <InputNumber :max="999999" :min="1" v-model="formValidate.inQty" style="width:100%"></InputNumber>
              </FormItem>
            </Col>
            <Col span="6">
              <FormItem label="单位" prop="procurementUnit">
                <Input
                  v-model="formValidate.procurementUnitValue"
                  placeholder="请输入"
                  disabled="disabled"
                />
              </FormItem>
            </Col>
            <Col span="6">
              <FormItem label="采购单价" prop="inPrice">
                <InputNumber :min="0" v-model="formValidate.inPrice" style="width:100%"></InputNumber>
              </FormItem>
            </Col>
            <Col span="6">
              <FormItem label="销售单价" prop="salePrice">
                <InputNumber :min="0" v-model="formValidate.salePrice" style="width:100%"></InputNumber>
              </FormItem>
            </Col>
          </Row>
          <FormItem label="指定采购单" prop="selectedPurchasId" v-if="priceDetail.isGroupAdd">
            <!-- <Select
              v-model="formValidate.selectedPurchasId"
              v-if="isMerge"
              placeholder="请选择指定新增到采购单"
            >
              <Option
                v-for="item in mergeRecordFilterList"
                :value="item.id"
                :key="item.id"
              >{{ item.description }}</Option>
            </Select> -->
            <Select v-model="formValidate.selectedPurchasId" placeholder="请选择指定新增到采购单">
              <Option v-for="item in applyIdList" :value="item.applyId" :key="item.applyId">{{ item.name }}</Option>
            </Select>
          </FormItem>
        </Form>
        <div slot="footer">
          <Button type="primary" @click="saveAddGoods('formValidateRef')" :loading="handleBtn">新增</Button>
          <Button type="default" @click="goodsFlag=false;handleReset('formValidateRef')">取消</Button>
        </div>
      </Modal>

      <!-- 加载所有物品 -->
      <Modal v-model="selectModal" width="800">
        <p slot="header" style="text-align: center;">选择项目</p>
        <Row>
          <Col span="24">
            <Input placeholder="检索..." v-model="modalSearch" ref="searchInput"/>
            <Spin size="large" fix v-if="listShow || !goodsFilter"></Spin>
            <ul class="liList">
              <!-- <li v-for="item in goodsFilter" :key="item.Id" @click="selectOneInfo(item)">
                <div>{{item.itemName}}</div>
              </li> -->
              <template v-for="item in goodsFilter">
                <li v-if="!item.isDisabled" :key="item.Id" @click="selectOneInfo(item)"><div>{{item.itemName}}</div></li>
                <li v-else :key="item.Id" class="disabledClick"><div>{{item.itemName}}</div></li>
              </template>
            </ul>
          </Col>
        </Row>
        <div slot="footer">
          <span style="float: left">
            <span>共{{goodsFilter.length}}项 </span>
            <span v-if="selectedItemType==1">[药品类型]</span>
            <span v-else-if="selectedItemType==2">[耗材类型]</span>
            <span v-else-if="selectedItemType==3">[固定资产类型]</span>
            <span v-else-if="selectedItemType==4">[低值易耗类型]</span>
            <span> 物品数据</span>
          </span>
          <Button @click="selectModal=false;modalSearch=''">关闭</Button>
        </div>
      </Modal>

      <!-- 删除物品  cy更新： 变成拒绝操作了-->
      <Modal v-model="delectModal" @on-cancel="delDatas.delRemark=''">
        <p slot="header" style="text-align: center;">确定要拒绝物品：{{delDatas.delName}} 吗？</p>
        <Input type="text" v-model="delDatas.delRemark" placeholder="请输入操作备注..."/>
        <div slot="footer">
          <Button type="error" @click="handleDel" :loading="handleBtn">拒绝</Button>
          <Button @click="delectModal=false;delDatas.delRemark='';handleBtn=false">关闭</Button>
        </div>
      </Modal>

      <!-- 20200623 拆分物品明细的数量 -->
      <Modal v-model="cutModal">
        <p slot="header" style="text-align: center;">确定拆分物品：{{cutDatas.name}} 吗？</p>
        <span style="margin-left:100px;">采购总数量：{{cutDatas.approvalQty}} </span>
        <span style="margin-left:100px;">拆分：<InputNumber :min="1" :max="cutDatas.approvalQty - 1" v-model="cutDatas.num" style="width:100px;"></InputNumber></span>
        <div slot="footer">
          <Button type="error" @click="handleCut" :loading="handleBtn">确定</Button>
          <Button @click="cutModal=false;handleBtn=false">关闭</Button>
        </div>
      </Modal>

      <!-- 生成订单 -->
      <Modal v-model="creatModal" width="800">
        <p slot="header" style="text-align: center;">生成采购订单</p>
        <Form
          ref="recordValidateRef"
          :model="recordValidate"
          :rules="recordRuleValidate"
          :label-width="90"
          style="margin-right: 30px;">
          <Row>
            <Col span="12">
              <FormItem label="申请人" prop="applyMain">
                <Input v-model="recordValidate.applyMain" disabled/>
              </FormItem>
            </Col>
            <Col span="12">
              <FormItem label="申请时间" prop="applyDate">
                <DatePicker
                  v-model="recordValidate.applyDate"
                  placement="bottom-end"
                  placeholder="请选择日期"
                ></DatePicker>
              </FormItem>
            </Col>
          </Row>
          <Row>
            <Col span="12" v-if="!isMerge">
              <FormItem label="合并类型" prop="catalogue">
                <Cascader
                  v-model="cataArr"
                  :data="catalogList"
                  :disabled="!isMerge"
                  trigger="hover"
                  placeholder="请选择合并类型"
                  style="width:170px;display: inline-table;"
                ></Cascader>
              </FormItem>
            </Col>
            <Col span="12" v-if="!isMerge">
              <FormItem label="合并供应商" prop="supplierId">
                <Select v-model="recordValidate.supplierId" placeholder="请选择供应商" filterable>
                  <!-- <Option value="0">全部</Option> -->
                  <Option
                    v-for="type in supplierFilteredList"
                    :key="type.id"
                    :value="type.id"
                  >{{type.name}}</Option>
                </Select>
              </FormItem>
            </Col>
          </Row>
          <Row>
            <Col span="24">
              <FormItem label="备注" prop="remark">
                <Input
                  v-model="recordValidate.remarks"
                  placeholder="请输入备注"
                  type="textarea"
                  :rows="3"
                />
              </FormItem>
            </Col>
          </Row>
        </Form>
        <div slot="footer">
          <span v-show="supplierFilteredList.length>1" style="float: left;color: red;line-height: 30px;">提示：存在多个供应商的物品</span>
          <Button
            type="primary"
            @click="handleCreateRecord('recordValidateRef')"
            :loading="handleBtn"
          >确定</Button>
          <Button @click="creatModal=false;handleReset('recordValidateRef')">关闭</Button>
        </div>
      </Modal>

      <!-- 拆单 -->
      <Modal v-model="detachFlag" width="435">
        <p slot="header" style="text-align: center;">确定要对合并采购单{{priceDetail.purchaseNo}}进行拆单吗？</p>
        <div style="margin:0 35px;">
          <span>拆分单号：</span>
          <Select v-model="detachGroupId" style="width:260px;" placeholder="请选择拆分的采购单">
            <Option v-for="item in applyIdList" :value="item.applyId" :key="item.applyId">{{ item.name }}</Option>
          </Select>
        </div>
        <div slot="footer">
          <Button type="error" @click="detachGroup" :loading="detachBtn">拆单</Button>
          <Button @click="detachFlag=false;detachGroupId='';">关闭</Button>
        </div>
      </Modal>
    </div>
  </transition>
</template>
<script>
import merge from './merge.js'
import { accAdd, accMul, toFilterKey, toFilterList } from '@/libs/tools.js'
import { setTimeout } from 'timers'
const BUTTONROLE = {
  CGSQD_DYSQD: 'CGSQD_DYSQD',
  CGSQD_DC: 'CGSQD_DC',
  CGSQD_XG: 'CGSQD_XQXG',
  CGSQD_SC: 'CGSQD_XQSC',
  CGSQD_SCDD: 'CGSQD_SCDD',
  CGSQD_XZWP: 'CGSQD_XZWP',
  CGSQD_SHENPI: 'CGSQD_SHENPI'
}
export default {
  mixins: [merge],
  data () {
    return {
      multiSelectFlag: false, // cy 20200623 开启生成订单多选模式的flag
      multiSelections: [],
      addGroupId: '', // cy 选中的采购单id：用于合并采购单的新增物品到该采购单
      detachBtn: false, // 拆单按钮loading
      detachFlag: false, // cy 拆单标识
      detachGroupId: '', // cy 选中的拆单项id
      // 状态
      stateList: [
        {
          id: '2',
          label: '未审批',
          color: 'default'
        },
        {
          id: '3',
          label: '已同意',
          color: 'success'
        },
        {
          id: '4',
          label: '已拒绝',
          color: 'error'
        },
        {
          id: '5',
          label: '已回退',
          color: 'primary'
        },
        {
          id: '6',
          label: '上级待审',
          color: 'success'
        },
        {
          id: '7',
          label: '暂 缓',
          color: 'default'
        }
      ],
      situationList: [
        { id: '3', name: '同意' },
        { id: '4', name: '拒绝' },
        { id: '5', name: '回退' },
        { id: '7', name: '暂缓' }
      ],
      cataArr: [], // cy 级联菜单数据
      selectedItemType: '', // cy table 物品类型
      selectedItemName: '', // cy table 物品类型名
      // isShowCreatBtn: true,
      recordValidate: {
        supplierId: '', // cy 0708新增供应商id
        applyId: [],
        // orderArtificialNo: '',
        applyDate: new Date(),
        remarks: '',
        applyMain: '',
        catalogue: ''
      },
      recordRuleValidate: {
        applyDate: [
          { required: true, type: 'date', message: '时间不能为空', trigger: 'blur' }
        ],
        remarks: [
          { required: true, message: '采购单价不能为空', trigger: 'blur' }
        ],
        applyMain: [
          { required: true, message: '申请人不能为空', trigger: 'blur' }
        ],
        catalogue: [
          { required: true, message: '合并类型不能为空', trigger: 'blur' },
          { message: '合并类型不能为空', trigger: 'change' }
        ],
        supplierId: [
          { required: true, message: '合并供应商不能为空', trigger: 'blur' },
          { message: '合并供应商不能为空', trigger: 'change' }
        ]
      },
      creatModal: false,
      mergeData: {},
      mergeLoading: false,
      // mergeRecordFilterList: [], // 合并的采购单记录list(未审批状态)
      mergeRecordList: [], // 合并的采购单list
      handleBtn: false,
      listShow: false,
      delectModal: false,
      cutModal: false,
      cutDatas: {
        id: '',
        name: '',
        approvalQty: 0,
        num: 1
      },
      delDatas: {
        delName: '',
        delRemark: '', // 删除备注
        delId: ''
      },
      modalSearch: '', // 搜索词
      selectModal: false,
      goodsList: [],
      goodsFlag: false,
      formValidate: {
        id: '',
        itemName: '',
        inQty: 1,
        inPrice: 0,
        salePrice: 0,
        // specificationsValue: '',
        procurementUnitValue: '',
        procurementUnit: '',
        procurementPackage: '',
        selectedPurchasId: '' // cy 合并采购单时新增的项目绑定到指定的采购单
      },
      ruleValidate: {
        itemName: [
          { required: true, message: '物品不能为空', trigger: 'blur' }
        ],
        inQty: [
          {
            required: true,
            type: 'number',
            message: '数量不能为空',
            trigger: 'blur'
          }
        ],
        inPrice: [
          {
            required: true,
            type: 'number',
            message: '采购单价不能为空',
            trigger: 'blur'
          }
        ],
        salePrice: [
          {
            required: true,
            type: 'number',
            message: '销售单价不能为空',
            trigger: 'blur'
          }
        ],
        selectedPurchasId: [
          { required: true, message: '指定采购单不能为空', trigger: 'blur' },
          { message: '指定采购单不能为空', trigger: 'change' }
        ]
      },
      purchase_detail: false,
      // 表格
      table_columns: [],
      // cy 固定资产 换特殊表头
      gdzc_columns: [
        {
          title: '序号',
          key: 'no',
          width: 40,
          align: 'center'
        },
        {
          title: '机构',
          key: 'centerName',
          tooltip: true,
          minWidth: 50
        },
        {
          title: '物资名称',
          key: 'medicalItemName'
        },
        {
          title: '件比',
          key: 'medicalThan',
          minWidth: 90
        },
        {
          title: '单位',
          key: 'packageUnitValue',
          width: 60,
          render: (h, params) => {
            return h('div', params.row.medicalItemRecordOutPut.packageUnitValue)
          }
        },
        {
          title: '采购数量',
          slot: 'approvalQty',
          key: 'approvalQty',
          width: 80
        },
        {
          title: '单价预算',
          slot: 'advicePrice',
          key: 'advicePrice',
          width: 80,
          align: 'center'
        },
        {
          title: '规格型号或主要技术参数',
          key: 'specModelsOrMainTechParams',
          minWidth: 100,
          align: 'center'
        },
        {
          title: '申请理由',
          // slot: 'appReason',
          tooltip: true,
          key: 'appReason'
        },
        {
          title: '供应商',
          key: 'supplierName',
          slot: 'supplierName',
          minWidth: 120
        },
        {
          title: '备注',
          slot: 'remark',
          tooltip: true,
          key: 'remark'
        }
      ],
      table_operate: [
        {
          title: '操作',
          slot: 'action',
          align: 'center',
          fixed: 'right',
          width: 110
        }
      ],
      table_data: [],
      loading: false,

      editIndex: -1,
      editAdvicePrice: 0, // 固定资产的单价预算
      editApprovalQty: 0,
      editInPrice: 0,
      editSalePrice: 0,
      editDualPrice: 0,
      editSupplier: '0',
      editRemark: '', // 备注
      approvalModal: false,
      approvalListId: '',
      approval: {
        situation: 3,
        advice: ''
      },
      saveLoading: false,
      emptyPrice: false,
      priceDetail: {},
      // 采购单流程
      approvalProcessOutPuts: [],
      // 按钮权限
      buttonRole: BUTTONROLE,
      // 表格高度
      tableHeight: 0,
      supplierFilteredList: [] // cy 供应商过滤
    }
  },
  props: {
    // cy 合并类型
    mergeType: {
      require: true,
      default: 1
    },
    catalogList: {
      require: true,
      default: []
    },
    selectedTypeId: {
      require: true,
      default: () => {
        return '0'
      }
    },
    formDetail: {
      require: true,
      default: () => {
        return {}
      }
    },
    supplierInfo: {
      require: true,
      default: () => {
        return {}
      }
    },
    supplierList: {
      require: true,
      default: []
    },
    idsList: {
      default: []
    },
    // mergeTypeId: {
    //   default: ''
    // },
    isMerge: {
      type: Boolean,
      default: false
    }
  },
  created () {
    this.recordValidate.applyMain = sessionStorage.getItem('empName')
  },
  computed: {
    isLowValueType(){
      return [this.priceDetail.catalogueName, this.selectedItemName].includes('低值易耗')
    },
    isShowCreatBtn () {
      // cy 20200623 增加 多选 ，点击过后不能再点击
      // cy 更新：全部已生成的状态由orderType的值来判断
      // this.isShowCreatBtn = this.priceDetail.orderType !== 2
      // cy 更更新：只有审核同意了才能显示生成订单 或者 合并的单子
      // cy 更更更新：类型!='低值易耗'
      return !this.multiSelectFlag && this.selectedItemName !='低值易耗' && (this.isMerge || (this.priceDetail.groupAuditStatus === '3' && this.priceDetail.orderType !== 2))
    },
    applyIdList () {
      let list = []
      if (this.mergeType === 2 || this.priceDetail.isGroupAdd) {
        // cy 如果是合并采购单或直接由合并采购单点击查看进入的明细页面，则计算出该合并单的具体采购单的id，用于拆单功能
        for (let res of this.table_data) {
          if (res.id === null) list.push({applyId: res.applyId, name: `${res.centerName}[${res.medicalItemName}]`})
        }
      }
      return list
    },
    currentStep () {
      if (this.approvalProcessOutPuts.length > 0) {
        return this.approvalProcessOutPuts[0].current - 1
      } else {
        return 0
      }
    },
    currentStatus () {
      if (this.priceDetail.groupAuditStatus === '4') {
        return 'error'
      } else {
        return 'process'
      }
    },
    goodsFilter () {
      let data = []
      if (this.goodsList) {
        data = toFilterKey(this.goodsList, 'medicalItemType', this.selectedItemType)
        data = toFilterKey(
          data,
          'medicalItemName,aliasName,manufacturer,packaging,mnemonic',
          this.modalSearch
        )
      } else {
        data = []
      }
      return data
    },
    tableColumns () {
      // console.log('tableColumns,  selectedItemType:', this.selectedItemType)
      let columns = []
      let hasEdit = this._filterButton(this.buttonRole.CGSQD_XG)
      let hasDelete = this._filterButton(this.buttonRole.CGSQD_SC)
      // cy orderType不为全部已生成，审批状态只能在未审批，并且有删除或编辑权限才能进行操作
      if (this.priceDetail.orderType !== 2 && this.priceDetail.groupAuditStatus === '2' && (hasEdit || hasDelete)) {
        if (this.selectedItemType === '3') {
          columns = [...this.gdzc_columns, ...this.table_operate]
        } else {
          columns = [...this.table_base, ...this.table_operate]
        }
      } else {
        if (this.selectedItemType === '3') {
          columns = [...this.gdzc_columns]
        } else {
          columns = [...this.table_base]
        }
      }
      if (this.multiSelectFlag) {
        columns.unshift({
          type: 'selection',
          width: 32,
          align: 'center'
        })
      } else {
        if (columns[0].type === 'selection') columns.shift()
      }
      return columns
    }
  },
  watch: {
    selectedTypeId () {
      if (this.catalogList.length !== 0 && this.selectedTypeId) {
        let arr = []
        arr = this.catalogList.filter(item => {
          if (item.hasOwnProperty('children') && item.children.length !== 0) {
            return item.children.some(res => { return res.value === this.selectedTypeId })
          } else {
            return item.value === this.selectedTypeId
          }
        })
        this.selectedItemType = arr.length ? arr[0].index : '0'
        this.selectedItemName = arr.length ? arr[0].label : '/'
      }
    },
    formDetail () {
      this.isMerge && this.$nextTick(() => {
        this.tableHeight = this.$refs.detail_table.clientHeight - 50
      })
    },
    // cy 合并的采购单明细数据
    mergeData () {
      // let filterList = []
      // if (this.mergeData.hasOwnProperty('purchaseRequest')) {
      //   this.mergeData['purchaseRequest'].forEach(item => {
      //     // cy 只保存状态为未审批的采购单
      //     if (item.groupAuditStatus === '2') {
      //       let description = `采购单${item.purchaseNo}，状态[未审批] ${
      //         item.auditor ? '审核人：' + item.auditor : ''
      //       }，采购总量：${item.totalQty}，机构：${
      //         item.centerId
      //       }，备注：（${item.remark || '无'}）  `
      //       filterList.push({ id: item.id, description: description })
      //     }
      //   })
      // }
      // cy list：新增物品到未审批的的采购单里去
      // this.mergeRecordFilterList = filterList

      // cy 表数据
      this.table_data = this.mergeData.purchaseDetails
      // this.mergeRecordList = this.mergeData.purchaseRequest
      // cy 合计的数据
      this.priceDetail = this.mergeData.totalMergeDetail
      // this.showCreatBtn()
      // this.filterSuppiler()
    }
  },
  methods: {
    // cy 给表单添加禁用状态
    setChecked () {
      let objData = this.$refs.detailTable.objData
      for (let index in objData) {
        // 初始化禁用、已勾选状态
        objData[index]._isDisabled = false
        objData[index]._isChecked = false
        // dataState为2表示被拒绝，orderNo为空表示未生成订单的采购明细
        if (objData[index].dataState === 2 || objData[index].orderNo !== '') objData[index]._isDisabled = true
      }
    },
    handleSelect (selection) {
      this.multiSelections = selection
    },
    detachGroup () {
      if (this.detachGroupId === '') {
        this.$Message.info(`请选择您要拆分的采购单！`)
        return false
      }
      this.detachBtn = true
      this.swsApi
        .swsPost(`CenterDocking/Purchase/MergePurchaseBreak`, {mergePurchaseId: this.priceDetail.id, applyId: this.detachGroupId})
        .then(res => {
          this.detachBtn = false
          if (res.data.success) {
            this.$Message.success(`拆分成功！`)
            this.detachFlag = false
            this.detachGroupId = ''
            if (this.applyIdList.length > 2) this.getFormDetail()
            // else {
            //   this.$emit('on-hide')
            //   this.$emit('on-refresh')
            // }
            else this.hide()
          } else this.$Message.error(`拆分失败，请稍后再试。`, e)
        })
        .catch(e => {
          this.detachBtn = false
          this.$Message.error(`请求失败，请稍后再试。`, e)
        })
    },
    // filterSuppiler () {
    //   // 20191206 cy 过滤供应商
    //   this.$nextTick(() => {
    //     if (this.supplierList.length > 0) {
    //       let sFL = []
    //       sFL = toFilterList(this.multiSelections, this.supplierList, 'supplierId', 'id')
    //       this.supplierFilteredList = toFilterList(this.priceDetail.purchaseToOrder, sFL, 'supplierId', 'id', false)
    //     }
    //   })
    // },
    // cy 设置采购申请明细的row样式
    setRejectRowColor (row) {
      if (row.dataState === 2) {
        return 'table-reject-row'
      } else if (row.isGroupAdd) {
        return 'table-groupAdd-row'
      } else if (!row.id) {
        return 'table-strongFont-row'
      }
    },
    // cy 根据采购单的订单状态【purchaseRequest的medicaItemTypeCount和purchaseToOrder】来计算生成订单按钮的显隐
    showCreatBtn () {
      // cy 更新：全部已生成的状态由orderType的值来判断
      // this.isShowCreatBtn = this.priceDetail.orderType !== 2
      // cy 更更新：只有审核同意了才能显示生成订单 或者 合并的单子
      this.isShowCreatBtn = this.isMerge || (this.priceDetail.groupAuditStatus === '3' && this.priceDetail.orderType !== 2)
    },
    // cy 生成订单
    handleCreateRecord (name) {
      let params = {
        ...this.recordValidate,
        supplierId: this.isMerge ? this.supplierInfo.supplierCheckedId : this.recordValidate.supplierId,
        applyId: this.idsList.length !== 0 ? this.idsList : [this.priceDetail.id],
        catalogue: this.cataArr.length ? this.cataArr[this.cataArr.length - 1] : '',
        detailId: this.multiSelections.length === 0 ? [] : this.multiSelections.map(res => {
          return res.id
        })
      }
      this.$refs[name].validate(valid => {
        // cy 新增供应商过滤
        if (valid) {
          this.handleBtn = true
          this.swsApi
            // .swsPost('CenterDocking/PurchaseDetail/PurchasToOrder', params)
            .swsPost('CenterDocking/NewPurchasToOrder/PurchasToOrder', params)
            .then(res => {
              if (res.data.success) {
                this.$Message.success(`生成订单成功！`)
                // 取消 多选
                this.multiSelectFlag = false
                this.dataUpdate()
                this.creatModal = false
                this.$router.push({ name: 'purchase_requisitions' })
              } else {
                this.$Message.error(res.data.error)
              }
              this.handleBtn = false
            })
            .catch(e => {
              this.$Message.error(`${e}，请稍后再试！`)
              this.handleBtn = false
            })
        } else {
          this.$Message.error('请仔细填写表格!')
        }
      })
    },
    // cy 打印
    printDetailTable () {
      // cy 增加打印类型
      let arr = this.catalogList.filter(res => { return res.index === this.selectedItemType })
      this.priceDetail.medicalTypeName = arr.length !== 0 ? arr[0].name : ''
      // cy 自定义多选的id数据无法预计，不能采用通过url传递参数的方式，解决方案有二，1.vuex共享数据+持久化存储，2.直接数据存sessionStorage
      sessionStorage.setItem('purchaseData', JSON.stringify(this.table_data))
      sessionStorage.setItem('purchaseDetail', JSON.stringify(this.priceDetail))
      let url = window.location.href.split('#')[0]
      window.open(`${url}#/print/purchase_print`)
    },
    // cy 导出
    exportDetailTable () {
      let tableData = [...this.table_data]
      // cy 在导出的表格里添加合计项 （不如直接让他们自己在excel里手动求和算了.）
      let datas = JSON.parse(JSON.stringify(tableData)).map(res => {
        // 20200514 cy 处理导出数据中有英文逗号而导致英文逗号后面的数据换到下一列中的问题
        if (typeof res.manufacturer === 'string' && res.manufacturer.indexOf(',') !== -1) {
          res.manufacturer = res.manufacturer.replace(',', ' ')
        }
        return res
      })
      datas.push({
        medicalItemName: '合计',
        inQty: this.priceDetail.totalQty,
        totalInPrice: this.priceDetail.actualPrice,
        totalSalePrice: this.priceDetail.salesTotalPrice
      })
      let columnsObj = {}
      this.table_base.forEach(item => {
        columnsObj[item.key] = item.title
      })
      datas.unshift(
        { inPrice: `采购单${this.priceDetail.purchaseNo || '合并明细表'}` },
        columnsObj
      )
      this.$refs.detailTable.exportCsv({
        filename: `采购单${this.priceDetail.purchaseNo || '合并明细表'}`,
        columns: this.table_base,
        noHeader: true,
        data: datas
      })
    },
    // cy 展示合并采购单详情
    showMergeDetail () {
      this.mergePurchaseRecord(this.idsList)
      this.purchase_detail = true
      this.table_data = []
    },
    // cy 加载所有物品
    loadGoods () {
      let that = this
      this.selectModal = true
      let params = {
        pageSize: 9999,
        pageNum: 1
      }
      // 查询物品
      // cy 减少请求次数
      if (!this.goodsList.length) {
        this.listShow = true
        this.swsApi
          .swsPost('Data/MedicalItemRecord/list', params)
          .then(function (res) {
            if (res.data.success) {
              let arr = []
              // 排除诊疗项目 and 禁用状态的 物品
              let result = res.data.result.filter(res => {
                return res.medicalItemType !== 5 && res.applyState === 1
              })
              // cy 新增过滤采购单里已经存在的物品
              let tableDataIds = that.table_data.map(res => res.medicalItemRecordOutPut.id)
              for (let i in result) {
                let item = {}
                // cy 新增过滤采购单里已经存在的物品
                item.isDisabled = tableDataIds.includes(result[i].id)
                // 物品名 + 别名 + 生产厂家 + 包装规格 +助记码 + 规格（minDose）
                item.itemName = `${result[i].medicalItemName}${result[i].brand
                  ? '(' + result[i].brand + ')' : ''}${result[i].aliasName
                  ? '(' + result[i].aliasName + ')' : ''}${result[i].manufacturer
                  ? '(' + result[i].manufacturer + ')' : ''}${result[i].packaging
                  ? result[i].packaging : ''}${result[i].minDose
                  ? result[i].minDose : ''}[${result[i].mnemonic}]`

                item.id = result[i].id
                // cy 用于模糊搜素
                item.medicalItemName = result[i].medicalItemName // 物品名称
                item.aliasName = result[i].aliasName // 别名
                item.manufacturer = result[i].manufacturer // 生产厂家
                item.packaging = result[i].packaging // 包装规格
                item.packageUnit = result[i].packageUnit // 包装单位ID
                item.mnemonic = result[i].mnemonic // 助记码
                item.medicalItemType = result[i].medicalItemType // 物品类型

                // cy 选取部分数据用于表格填写的赋值
                item.isSplited = result[i].isSplited // 单位
                item.packageUnitValue = result[i].packageUnitValue // 单位
                item.specificationsValue = result[i].specificationsValue // 单位
                item.purchasingPrice =
                  result[i].medicalDrugExtension.purchasingPrice // 采购价
                item.retailPrice = result[i].medicalDrugExtension.retailPrice // 销售价
                arr.push(item)
              }
              that.goodsList = arr
              that.listShow = false
            }
          })
      } else {
        that.listShow = false
      }
    },
    // cy 选中一个物品
    selectOneInfo (item) {
      this.formValidate.id = item.id
      this.formValidate.inQty = 1
      this.formValidate.itemName = item.itemName
      this.formValidate.inPrice = item.purchasingPrice
      this.formValidate.salePrice = item.retailPrice
      // this.formValidate.specificationsValue = item.specificationsValue
      this.formValidate.procurementUnitValue = item.isSplited === 1 ? item.specificationsValue : item.packageUnitValue
      // this.formValidate.procurementUnit = item.packageUnit
      // this.formValidate.procurementPackage = item.packaging
      this.formValidate.procurementUnit = item.isSplited === 1 ? item.specifications : item.packageUnit
      this.formValidate.procurementPackage = item.isSplited === 1 ? item.specificationsValue : item.packageUnitValue
      this.modalSearch = ''
      this.goodsFlag = true
      this.selectModal = false
    },
    // cy保存新增物品
    saveAddGoods (name) {
      let {
        id,
        selectedPurchasId,
        inQty,
        procurementPackage,
        procurementUnit,
        inPrice,
        salePrice
      } = { ...this.formValidate }
      let params = {
        applyId: this.priceDetail.isGroupAdd ? selectedPurchasId : this.priceDetail.id, // cy 新增物品分为两种情况 1.查看详情时的新增  2.合并采购单时将物品新增到选择的未审批状态单
        medicalItemId: id,
        inQty: inQty,
        procurementPackage: procurementPackage,
        procurementUnit: procurementUnit,
        // procurementPackage: procurementPackage,
        // procurementUnit: procurementUnit,
        inPrice: inPrice,
        salePrice: salePrice
      }
      this.$refs[name].validate(valid => {
        if (valid) {
          this.handleBtn = true
          this.swsApi
            .swsPost('CenterDocking/PurchaseDetail/Add', params)
            .then(res => {
              if (res.data.success) {
                this.goodsFlag = false
                this.$Message.success(`添加成功！`)
                // 1.查看详情时的新增  2.合并采购单时选择的未审批状态单的 新增项目
                // cy 新增后的 刷新
                this.dataUpdate()
                // cy 将已添加成功的物品加上禁选标识
                this.goodsList.forEach(item => {
                  if (item.id === id) item.isDisabled = true
                })
              } else {
                this.$Message.error(res.data.error)
              }
              this.handleBtn = false
            })
            .catch(e => {
              this.$Message.error(`添加请求失败,`, e)
              this.handleBtn = false
            })
        } else {
          this.$Message.error('请仔细填写表格!')
        }
      })
    },
    cutBtn (row, index) {
      this.cutModal = true
      this.cutDatas.id = row.id
      this.cutDatas.name = row.medicalItemName
      this.cutDatas.approvalQty = row.approvalQty
    },
    handleCut () {
      this.handleBtn = true
      this.swsApi
        .swsPost('CenterDocking/PurchaseDetail/SplitAdd', {
          detailId: this.cutDatas.id,
          detailNum: this.cutDatas.num
        })
        .then(res => {
          if (res.data.success) {
            this.cutModal = false
            this.$Message.success(`拆分明细数量成功！`)
            this.cutDatas.num = 1
            this.dataUpdate()
          } else {
            this.$Message.error(`拆分明细数量操作失败，请稍后再试！`)
          }
          this.handleBtn = false
        })
        .catch(e => {
          this.$Message.error(`拆分明细数量请求失败，请稍后再试！`)
        })
    },
    // cy 点击删除按钮
    delBtn (row, index) {
      this.delectModal = true
      this.delDatas.delId = row.id
      this.delDatas.delName = row.medicalItemName
    },
    // cy 删除物品
    handleDel () {
      if (!this.delDatas.delRemark) {
        this.$Message.info(`请填写删除备注！`)
        return false
      }
      this.handleBtn = true
      this.swsApi
        .swsPost('CenterDocking/PurchaseDetail/del', {
          id: this.delDatas.delId,
          remark: this.delDatas.delRemark
        })
        .then(res => {
          if (res.data.success) {
            this.delectModal = false
            this.$Message.success(`删除成功！`)
            this.delDatas.delRemark = ''
            this.dataUpdate()
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
      this.handleBtn = false
    },
    handleEdit (row, index) {
      this.editIndex = index
      // this.editApplyReason = row.appReason || 0
      this.editAdvicePrice = row.advicePrice || 0
      this.editApprovalQty = row.approvalQty || 0
      this.editInPrice = row.inPrice || 0
      this.editSalePrice = row.salePrice || 0
      // this.editDualPrice = row.dualPrice || 0
      this.editSupplier = row.supplierId || '0'
      this.editRemark = row.remark || ''
    },
    // cy 当采购价改变时 同步更新 双控价
    inPriceChange (inprice) {
      this.editDualPrice += 10
      let price = 0
      if (inprice <= 10) price = accMul(inprice, 1.3)
      if (inprice > 10 && inprice <= 100) price = accMul(inprice, 1.25)
      if (inprice > 100 && inprice <= 300) price = accMul(inprice, 1.2)
      if (inprice > 300 && inprice <= 500) price = accMul(inprice, 1.15)
      if (inprice > 500) price = accAdd(inprice, 75)
      this.editDualPrice = parseFloat(price.toFixed(4))
    },
    dataUpdate () {
      console.log('1dataUpdate')
      if (this.isMerge) {
        this.mergePurchaseRecord(this.idsList)
      } else {
        this.getFormDetail()
      }
    },
    handleSave (row, index) {
      let paramsBase = {
        purchaseRequestId: row.applyId,
        itemsId: row.id,
        approvalQty: this.editApprovalQty,
        remark: this.editRemark
      }
      let params = {}
      // cy 根据分类配置保存参数
      if (this.selectedItemType === '3') {
        params = {
          ...paramsBase,
          inPrice: this.editAdvicePrice,
          // appReason: this.editApplyReason,
          supplierId: this.editSupplier
        }
      } else {
        params = {
        // cy 新增采购单id
          ...paramsBase,
          inPrice: this.editInPrice,
          salePrice: this.editSalePrice,
          // dualPrice: this.editDualPrice,
          supplierId: this.editSupplier
        }
      }
      this.swsApi
        .swsPost('CenterDocking/UpdatePurchasPrice', params)
        .then(res => {
          if (res.data.success) {
            this.$Message.success(`调整成功！`)
            this.dataUpdate()
          } else {
            this.$Message.error(`操作失败，请稍后再试！`)
          }
          this.editIndex = -1
        })
        .catch(e => {
          this.editIndex = -1
        })
    },
    show () {
      this.purchase_detail = true
      this.table_data = []
      setTimeout(() => {
        this.getFormDetail()
      }, 300)
    },
    handleFormData (res) {
      if (res.data.success) {
        res.data.result.purchaseDetails.forEach(i => {
          let {
            medicalItemName,
            medicalItemType, // cy 新增物品类别
            minDose,
            manufacturer,
            brand,
            procurementUnitValue,
            medicalThan
          } = i.medicalItemRecordOutPut
          i.supplierName = i.id ? i.supplierName : ''
          // 将这三个数据提出来，为了避免render造成csv导出该对应数据为空的问题
          i.medicalItemName = `${medicalItemName}${brand ? '(' + brand + ')' : ''}`
          i.procurementUnitValue = procurementUnitValue
          i.minDose = minDose
          i.manufacturer = manufacturer
          // 统计每种物品的采购、销售总金额
          // i.totalInPrice = accMul(i.approvalQty || 0, i.inPrice || 0)
          // i.totalSalePrice = accMul(i.approvalQty || 0, i.salePrice || 0)
          i.medicalItemType = medicalItemType
          i.medicalThan = medicalThan
        })
        this.table_data = res.data.result.purchaseDetails
        this.priceDetail = res.data.result.purchaseRequest
        this.approvalProcessOutPuts = res.data.result.approvalProcessOutPuts

        setTimeout(() => {
          this.tableHeight = this.$refs.detail_table.clientHeight - 50
          // this.filterSuppiler()
        }, 0)
        // cy
        // this.showCreatBtn()
      } else {
        this.$Message.error(`采购单详情查询出错，请稍后再试！`)
      }
    },
    getFormDetail () {
      this.loading = true
      this.swsApi
        .swsPost(`CenterDocking/NewPurchaseDetailList/${this.formDetail.id || this.priceDetail.id}`)
        .then(res => {
          this.handleFormData(res)
          this.loading = false
        })
        .catch(e => {
          this.loading = false
          this.$Message.error(`采购单详情查询出错，请稍后再试`, e)
        })
    },
    creatRecordBtn () {
      if (this.multiSelections.length < 1) {
        this.$Modal.warning({
          title: '提示',
          content: '<p>请至少选择一项进行生成订单操作！</p>'
        })
        return false
      }
      if (this.supplierList.length > 0) {
        // let sFL = []
        // sFL = toFilterList(this.multiSelections, this.supplierList, 'supplierId', 'id')
        // this.supplierFilteredList = toFilterList(this.priceDetail.purchaseToOrder, sFL, 'supplierId', 'id', false)
        this.supplierFilteredList = toFilterList(this.multiSelections, this.supplierList, 'supplierId', 'id')
      }
      this.showCreatModal()
    },
    // 生成订单
    showCreatModal () {
      // this.recordValidate.medicalItemType = this.selectedItemType // cy 设置初值
      this.recordValidate.catalogue = this.selectedTypeId // cy 设置初值
      this.cataArr = this.getCascaderParentId(this.selectedTypeId) // cy给级联菜单赋值上默认数据
      this.creatModal = true
    },
    getCascaderParentId (childId) {
      let obj = this.catalogList.find(res => {
        let flag = false
        if (res.hasOwnProperty('children') && res.children.length !== 0) {
          flag = res.children.some(re => { return re.value === childId })
        }
        return flag
      })
      return obj ? [obj.value, childId] : [childId]
    },
    // 审批
    showApproval () {
      this.approvalListId = this.priceDetail.id
      this.approvalModal = true
      this.emptyPrice = false
      this.table_data.forEach(obj => {
        obj.salePrice === 0 && (this.emptyPrice = true)
      })
    },
    saveApproval () {
      let params = {
        id: this.approvalListId,
        groupAdvice: this.approval.advice,
        groupAuditStatus: this.approval.situation
      }
      if (!this.approval.advice) {
        this.$Message.error(`请输入审批意见！`)
        return false
      }
      this.saveLoading = true
      this.swsApi
        .swsPost('CenterDocking/NewApprovalPurchaseDetail', params)
        .then(res => {
          if (res.data.success) {
            this.$Message.success(`审批完成！`)
            this.purchase_detail = false
            this.$emit('on-save')
            this.$emit('on-hide')
          } else {
            this.$Message.error(res.data.error)
          }
          this.approvalModal = false
          setTimeout(() => {
            this.saveLoading = false
          }, 100)
        })
        .catch(e => {
          this.approvalModal = false
          this.saveLoading = false
        })
    },
    hide () {
      // 取消 多选
      this.multiSelectFlag = false
      // 取消 重置合并的参数
      // 清空 采购单过滤列表
      this.selectedItemType = ''
      // this.mergeRecordFilterList = []
      this.purchase_detail = false

      this.editIndex = -1
      if (this.priceDetail.isGroupAdd) this.$emit('on-refresh')
      this.priceDetail = {}
      this.approvalProcessOutPuts = []
      this.$emit('on-hide')
    }
  }
}
</script>
<style scoped lang="less">
.fade_enter-enter-active {
  transition: opacity 0.3s;
}
.fade_enter-enter, .fade_enter-leave-to /* .fade-leave-active below version 2.1.8 */ {
  opacity: 0;
}
.purchase_detail {
  display: flex;
  flex-direction: column;
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 100%;
  padding: 20px;
  background: #ffffff;
  z-index: 4;
  overflow-y: auto;
  .top {
    .record {
      margin-top: 10px;
      // margin: 10px 0;
      color: #333333;
      font-size: 16px;
      span {
        margin-left: 10px;
      }
      .right {
        float: right;
        button {
          font-size: 13px;
        }
        .approval {
          background: #f90;
          color: #ffffff;
          border-color: #f90;
        }
      }
      &::after {
        display: block;
        content: '';
        height: 0;
        visibility: hidden;
        clear: both;
      }
    }
    .step-wrapper {
      width: calc(~'100% - 82px');
      display: inline-block;
      vertical-align: top;
      // margin-top: 10px;
      // padding-right: 20px;
    }
    .items-box {
      font-size: 14px;
      margin-top: 10px;
      .field {
        color: #666666;
        & + .red {
          color: #fc4b4b;
        }
      }
      span:not(.field) {
        font-weight: bold;
        margin-right: 30px;
      }
    }
    span {
      &.error {
        color: #ed4014;
      }
      &.success {
        color: #19be6b;
      }
      &.default {
        color: #f90;
      }
    }
  }
  .split-line {
    flex: 0 0 1px;
  }
  .detail_table {
    flex: 1;
    overflow: hidden;
    // overflow-y: scroll;
    .ivu-table-wrapper {
      border: none !important;
      & /deep/ .ivu-table-tip {
        overflow: hidden;
      }
      & /deep/ .ivu-table {
        td {
          color: #666;
        }
        &::before,
        &::after {
          display: none !important;
        }
      }
      & /deep/ .ivu-table th {
        font-size: 14px;
        background: #f9f9f9;
        // border-bottom: none;
      }
      & /deep/ .ivu-table .ivu-table-cell {
        padding-right: 5px;
        padding-left: 5px;
      }
      // & /deep/ .ivu-table .high_light_row td{
      //   background-color: #EBF7FF;
      // }
      & /deep/ .ivu-table .high_light {
        // background-color: #EBF7FF;
        color: red;
        font-size: 15px;
        font-weight: bold;
      }
      & /deep/ .ivu-table .table-reject-row td {
        background-color: #e8eaec;
      }
      & /deep/ .ivu-table .table-groupAdd-row td {
        background-color: #c9f3e2;
      }
      & /deep/ .ivu-table .table-strongFont-row td {
        font-size: 14px;
        color: #000;
        font-weight: bold;
      }
      & /deep/ .ivu-table .supplier-empty {
        color: red;
        font-size: 15px;
        font-weight: bold;
      }
    }
    .merge-btn-goups {
      float: left;
      margin-top: 16px;
    }
    .tips {
      float: right;
      margin-top: 16px;
      strong {
        font-size: 16px;
      }
    }
  }
  .button-group {
    button + button {
      margin-left: 10px;
    }
  }
}
.emptyPrice {
  color: red;
  margin-left: 60px;
}
.liList {
  max-height: 450px;
  overflow-y: scroll;
  li {
    height: 30px;
    line-height: 30px;
    font-size: 12px;
    border-bottom: 1px solid #eaeaea;
    cursor: pointer;
    white-space: nowrap;
    text-overflow: ellipsis;
    overflow: hidden;
    i {
      color: red;
    }
  }
  li:hover {
    background: #E8EAEC;
  }
  .disabledClick {
    cursor: not-allowed;
    color: #aaa;
  }
}
.goods_data {
  /deep/ .ivu-input-prefix i {
    font-size: 12px;
  }
}
.ivu-table .supplier-empty {
  background-color: #ff6600;
  color: #fff;
}
.ivu-divider-horizontal {
  margin: 10px 0;
}
</style>
