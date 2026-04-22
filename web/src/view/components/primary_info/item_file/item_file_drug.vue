/* eslint-disable camelcase */
<template>
  <div id="item-file">
    <item-file-template ref="item-file-template" :table-column="table_column" :table-column2="table_column2" :wareh-house-type-id="WAREHOUSEID" @show-add-modal="showAdd" @get-cascader="getCascader" @export-tb="exportTb" :importId="importId" :stopAutoLoad="true"></item-file-template>

    <Modal v-model="addFlag" width="800" @on-cancel="cancel('formValidate')" :mask-closable="false">
      <p slot="header" style="text-align: center;">{{title}}{{detailTitle}}档案</p>
      <Form ref="formValidate" class="form" :model="formValidate" :label-width="130" :rules="ruleValidate">
        <div>
          <p class="title-row">基本信息</p>
          <div class="basic-row">
            <Row>
              <Col span="24">
              <FormItem label="档案类别" prop="wareHouseId">
                <Cascader :data="cascaderData" v-model="formValidate.wareHouseId" :disabled="readonly" change-on-select @on-change="cascaderChange"></Cascader>
              </FormItem>
              </Col>
              <Col span="12">
              <label class="red-item-label">药品名称</label>
              <FormItem prop="medicalItemName">
                <Input placeholder="请输入" v-model="formValidate.medicalItemName" :readonly="readonly" :disabled="lock"></Input>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem label="药品编码" prop="medicalItemWorkCode">
                <Input placeholder="请输入" v-model="formValidate.medicalItemWorkCode" :readonly="readonly"></Input>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem label="品牌/型号" prop="brand">
                <Input placeholder="请输入" v-model="formValidate.brand" :readonly="readonly"></Input>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem label="商品名称" prop="goodsName">
                <Input placeholder="请输入" v-model="formValidate.goodsName" :readonly="readonly"></Input>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem label="别名" prop="aliasName">
                <Input placeholder="请输入" v-model="formValidate.aliasName" :readonly="readonly"></Input>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem label="英文名称" prop="englishName">
                <Input placeholder="请输入" v-model="formValidate.englishName" :readonly="readonly"></Input>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem label="助记码" prop="mnemonic">
                <Input placeholder="请输入" v-model="formValidate.mnemonic" :disabled="editLock" :readonly="readonly"></Input>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem prop="forms" label="剂型">
                <Cascader :data="Form" v-model="formValidate.forms" :disabled="readonly"></Cascader>
              </FormItem>
              </Col>
            </Row>
            <Row>
              <Col span="12">
              <FormItem prop="packaging" label="包装规格">
                <Input placeholder="请输入" v-model="formValidate.packaging" :readonly="readonly"></Input>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem prop="packageUnit" label="包装单位">
                <Select v-model="formValidate.packageUnit" filterable :disabled="readonly">
                  <Option :value="item.id" v-for="(item, i) in PackageUnit" :key="i">{{item.speUnitCHS}}</Option>
                </Select>
              </FormItem>
              </Col>
              <Col span="12">
              <label class="red-item-label">规格</label>
              <FormItem prop="minDose">
                <Input placeholder="请输入" v-model="formValidate.minDose" :readonly="readonly"></Input>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem label="规格单位" prop="specifications">
                <Select placeholder="请选择" filterable v-model="formValidate.specifications" :disabled="readonly">
                  <Option :value="item.id" v-for="(item, i) in Specifications" :key="i">{{item.speUnitCHS}}</Option>
                </Select>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem prop="approvalNum" label="浓度" class="medMIC">
                <Input placeholder="请输入" v-model="formValidate.concentration"></Input>
                <p class="medMICP">浓度不在名称中时填写</p>
              </FormItem>
              </Col>
              <Col span="12">
              <label class="red-item-label">生产厂家</label>
              <FormItem prop="manufacturer">
                <Input placeholder="请输入" :readonly="readonly" v-model="formValidate.manufacturer" :disabled="lock"></Input>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem prop="approvalNum" label="批准文号">
                <Input placeholder="请输入" :readonly="readonly" v-model="formValidate.approvalNum"></Input>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem prop="medicalThan" label="件比">
                <Input placeholder="请输入" v-model="formValidate.medicalThan" :readonly="readonly"></Input>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem prop="doseMin" label="最小剂量">
                <InputNumber placeholder="请输入" v-model="formValidate.doseMin" style="width:100%" :readonly="readonly"></InputNumber>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem prop="doseUnit" label="剂量单位">
                <Select v-model="formValidate.doseUnit">
                  <Option :value="item.id" v-for="(item, i) in DoseUnit" :key="i">{{item.speUnitUS}}</Option>
                </Select>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem prop="doseUnit" label="优先出库供应商">
                <Select v-model="formValidate.prioritySupplier" filterable>
                  <Option :value="item.id" v-for="(item, i) in SupplierId" :key="i">{{item.name}}</Option>
                </Select>
              </FormItem>
              </Col>
            </Row>
            <Row>
              <Col span="12">
              <FormItem prop="isSplited" label="是否可拆分">
                <RadioGroup v-model="formValidate.isSplited">
                  <Radio :label="1" :disabled="readonly">是</Radio>
                  <Radio :label="0" :disabled="readonly">否</Radio>
                </RadioGroup>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem prop="specificationsQuantity" label="包装拆分规则">
                <InputNumber placeholder="请输入" v-model="formValidate.specificationsQuantity" style="width:100%" :disabled="formValidate.isSplited!==1" :readonly="readonly"></InputNumber>
              </FormItem>
              </Col>
            </Row>
            <!-- 新增  mayCharges  是否可划价-->
            <Row>
              <Col span="12">
              <FormItem prop="mayCharges" label="是否可划价">
                <RadioGroup v-model="formValidate.mayCharges">
                  <Radio :label="1" :disabled="readonly">是</Radio>
                  <Radio :label="0" :disabled="readonly">否</Radio>
                </RadioGroup>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem prop="isSellFree" label="售价可为0">
                <i-switch v-model="formValidate.isSellFree" :disabled="readonly">
                  <template #open>
                    <span>是</span>
                  </template>
                  <template #close>
                    <span>否</span>
                  </template>
                </i-switch>
              </FormItem>
              </Col>
            </Row>
          </div>
          <p class="title-row">产品说明</p>
          <div class="basic-row">
            <Row>
              <Col span="12">
              <FormItem label="患者自然月可用量" prop="monthDosage">
                <InputNumber :min="0" class="w-100" :readonly="readonly" placeholder="请输入" v-model="formValidate.monthDosage" :disabled="editLock"></InputNumber>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem label="自然月人为损耗量" prop="artificialLoss">
                <InputNumber :min="0" class="w-100" :readonly="readonly" placeholder="请输入" v-model="formValidate.artificialLoss" :disabled="editLock"></InputNumber>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem label="自然年最大使用量" prop="yearDosage">
                <InputNumber :min="0" class="w-100" :readonly="readonly" placeholder="请输入" v-model="formValidate.yearDosage" :disabled="editLock"></InputNumber>
              </FormItem>
              </Col>
              <Col span="24">
              <FormItem label="适应症" prop="iindications">
                <Input placeholder="请输入" :readonly="readonly" type="textarea" v-model="formValidate.iindications"></Input>
              </FormItem>
              </Col>
              <Col span="24">
              <FormItem label="用法用量" prop="usage">
                <Input placeholder="请输入" :readonly="readonly" type="textarea" v-model="formValidate.usage"></Input>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem prop="feeTypeId" label="费用类别">
                <Select v-model="formValidate.feeTypeId" filterable :disabled="readonly">
                  <Option :value="item.id" v-for="(item, i) in FeeTypeId" :key="i">{{item.name}}</Option>
                </Select>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem label="警戒库存" prop="minInventory">
                <InputNumber :min="0" placeholder="请输入" v-model="formValidate.minInventory" :readonly="readonly" style="width:100%" :disabled="editLock"></InputNumber>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem prop="dataState" label="数据状态">
                <RadioGroup v-model="formValidate.dataState">
                  <Radio :label="1" :disabled="editLock">可用</Radio>
                  <Radio :label="2" :disabled="editLock">停用</Radio>
                </RadioGroup>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem prop="isSalesReturn" label="是否可退货">
                <RadioGroup v-model="formValidate.isSalesReturn">
                  <Radio :label="0" :disabled="readonly">不可退货</Radio>
                  <Radio :label="1" :disabled="readonly">可退货</Radio>
                </RadioGroup>
              </FormItem>
              </Col>
            </Row>
          </div>
          <p class="title-row">价格信息</p>
          <div class="basic-row">
            <Row>
              <Col span="12">
              <FormItem label="存储条件" prop="storageConditions">
                <Input placeholder="请输入" :readonly="readonly" v-model="formValidate.storageConditions"></Input>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem label="首批进货人员" prop="firstStock">
                <Input placeholder="请输入" :readonly="readonly" v-model="formValidate.firstStock"></Input>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem label="建议采购价" prop="medicalDrugExtension.purchasingPrice">
                <InputNumber :min="0" :readonly="readonly" class="w-100" placeholder="请输入" v-model="formValidate.medicalDrugExtension.purchasingPrice" :disabled="editLock" @on-blur="() => this.setDefaultPrice('purchasingPrice')"></InputNumber>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem label="协议价" prop="medicalDrugExtension.agreementPrice">
                <InputNumber :min="0" :readonly="readonly" class="w-100" placeholder="请输入" v-model="formValidate.medicalDrugExtension.agreementPrice" :disabled="editLock" @on-blur="() => this.setDefaultPrice('agreementPrice')"></InputNumber>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem label="参考价" prop="medicalDrugExtension.referencePrice">
                <InputNumber :min="0" :readonly="readonly" class="w-100" placeholder="请输入" v-model="formValidate.medicalDrugExtension.referencePrice" :disabled="editLock" @on-blur="() => this.setDefaultPrice('referencePrice')"></InputNumber>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem label="政府指导价" prop="medicalDrugExtension.gocGuidePrice">
                <InputNumber :min="0" :readonly="readonly" class="w-100" placeholder="请输入" v-model="formValidate.medicalDrugExtension.gocGuidePrice" :disabled="editLock" @on-blur="() => this.setDefaultPrice('gocGuidePrice')"></InputNumber>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem label="社保指导价" prop="medicalDrugExtension.socialSecurityPrice">
                <InputNumber :min="0" :readonly="readonly" class="w-100" placeholder="请输入" v-model="formValidate.medicalDrugExtension.socialSecurityPrice" :disabled="editLock" @on-blur="() => this.setDefaultPrice('socialSecurityPrice')"></InputNumber>
              </FormItem>
              </Col>
              <Col span="24">
              <FormItem label="备注" prop="remark">
                <Input placeholder="请输入" :readonly="readonly" type="textarea" v-model="formValidate.remark" :disabled="editLock"></Input>
              </FormItem>
              </Col>
            </Row>
          </div>
        </div>
        <Spin size="small" fix v-if="detailLoading"></Spin>
      </Form>
      <div slot="footer">
        <Button type="info" v-show="title === '新增'" @click="stickValue">黏贴</Button>
        <Button type="info" v-show="title === '查看'" @click="copyValue">复制</Button>
        <Button type="success" @click="showEdit(formValidate)" v-show="!saveModal" v-permission="buttonRole.WPDA_BJ">编辑</Button>
        <Button type="primary" @click="save" v-show="saveModal">保存</Button>
        <Button type="default" @click="cancel('formValidate')">取消</Button>
      </div>
    </Modal>
  </div>
</template>

<script>
import itemFileTemplate from "@/components/item-file-template";
import Operate from "@/components/operate";
import { deepClone } from "@/libs/tools.js";
import mixins from "./mixins.js";
const FEETYPEID = "31166b331f7b2596974b6977b9fd5ac2";
const WAREHOUSEID = 1;
const IMPORTID = 4;
export default {
  name: "item_file_drug",
  data() {
    return {
      importId: IMPORTID,
      treeData: [],
      wareHouseId: -1, // 库房id
      loading: false,
      detailLoading: false,

      // cy:editPriceModal
      // editPriceModal: false,

      item_data: [],
      addFlag: false,
      table_column: [
        {
          title: "名称",
          key: "medicalItemName",
          render: (h, params) => {
            return params.row.brand ? (
              <span>
                {params.row.medicalItemName}（{params.row.brand}）
              </span>
            ) : (
              <span>{params.row.medicalItemName}</span>
            );
          },
        },
        {
          title: "当前库存",
          key: "medInventory",
        },
        {
          title: "编码",
          key: "medicalItemCode",
        },
        {
          title: "助记码",
          key: "mnemonic",
          // maxWidth: 120
        },
        {
          title: "包装",
          key: "packaging",
        },
        {
          title: "建议采购价",
          key: "medicalDrugExtension.purchasingPrice",
          width: 80,
          render: (h, params) => {
            if (params.row.medicalDrugExtension.centerName === "统一价") {
              return (
                <span style="color:blue;">
                  {params.row.medicalDrugExtension.purchasingPrice || 0}元
                </span>
              );
            } else {
              return (
                <span>
                  {params.row.medicalDrugExtension.purchasingPrice || 0}元
                </span>
              );
            }
          },
        },
        {
          title: "建议销售价",
          key: "medicalDrugExtension.retailPrice",
          width: 80,
          render: (h, params) => {
            if (
              params.row.medicalDrugExtension.socialSecurityPrice !== 0 &&
              params.row.medicalDrugExtension.retailPrice >
                params.row.medicalDrugExtension.socialSecurityPrice
            ) {
              return (
                <span style="color:red;">
                  {params.row.medicalDrugExtension.retailPrice || 0}元
                </span>
              );
            } else {
              return (
                <span>
                  {params.row.medicalDrugExtension.retailPrice || 0}元
                </span>
              );
            }
          },
        },
        {
          title: "社保指导价",
          key: "medicalDrugExtension.socialSecurityPrice",
          width: 80,
          render: (h, params) => {
            return (
              <span>
                {params.row.medicalDrugExtension.socialSecurityPrice || 0}元
              </span>
            );
          },
        },
        {
          title: "费用类别",
          key: "feeTypeId",
          align: "center",
          width: 80,
          render: (h, params) => {
            let type = this.FeeTypeId.filter((item) => {
              return item.id === params.row.feeTypeId;
            });
            return <span>{type.length ? type[0].name : "无"}</span>;
          },
        },
        // {
        //   title:'浓度',
        //   key:'concentration',
        //   width: 80,
        // },
        {
          title: "生产厂家",
          key: "manufacturer",
          tooltip: true,
        },
        {
          title: "状态",
          key: "dataState",
          width: 80,
          render: (h, params) => {
            if (params.row.dataState === 1) {
              return <tag color="success">可用</tag>;
            } else {
              return <tag color="error">禁用</tag>;
            }
          },
        },
        {
          title: "采购状态",
          key: "applyState",
          width: 80,
          render: (h, params) => {
            if (params.row.applyState === 1) {
              return (
                <i-button
                  type="success"
                  size="small"
                  onClick={() => this.applyStateToggle(params.row)}
                >
                  可采购
                </i-button>
              );
            } else {
              return (
                <i-button
                  type="error"
                  size="small"
                  onClick={() => this.applyStateToggle(params.row)}
                >
                  禁止采购
                </i-button>
              );
            }
          },
        },
        {
          title: "操作",
          key: "action",
          width: 120,
          align: "center",
          fixed: "right",
          render: (h, params) => {
            return (
              <Operate
                textEdit="调价"
                handleDelete={() => this.showDel(params.row)}
                showDelete={params.row.medInventory <= 0}
                handleEdit={() => {
                  this.$refs["item-file-template"].showEditPrice(params.row);
                  delete this.formValidate.hiCenterCode;
                  delete this.formValidate.form;
                }}
                handleWatch={() =>
                  this.showDetail(params.row.id, params.row.medicalItemName)
                }
                permissionEdit={this.buttonRole.WPDA_XG}
                permissionDelete={this.buttonRole.WPDA_SC}
              />
            );
          },
        },
      ],
      table_column2: [
        {
          title: "名称",
          key: "medicalItemName",
          render: (h, params) => {
            return params.row.brand ? (
              <span>
                {params.row.medicalItemName}（{params.row.brand}）
              </span>
            ) : (
              <span>{params.row.medicalItemName}</span>
            );
          },
        },
        {
          title: "当前库存",
          key: "medInventory",
        },
        {
          title: "编码",
          key: "medicalItemCode",
        },
        {
          title: "助记码",
          key: "mnemonic",
          // maxWidth: 120
        },
        {
          title: "包装",
          key: "packaging",
        },
        {
          title: "建议采购价",
          key: "medicalDrugExtension.purchasingPrice",
          width: 80,
          render: (h, params) => {
            if (params.row.medicalDrugExtension.centerName === "统一价") {
              return (
                <span style="color:blue;">
                  {params.row.medicalDrugExtension.purchasingPrice || 0}元
                </span>
              );
            } else {
              return (
                <span>
                  {params.row.medicalDrugExtension.purchasingPrice || 0}元
                </span>
              );
            }
          },
        },
        {
          title: "建议销售价",
          key: "medicalDrugExtension.retailPrice",
          width: 80,
          render: (h, params) => {
            if (
              params.row.medicalDrugExtension.socialSecurityPrice !== 0 &&
              params.row.medicalDrugExtension.retailPrice >
                params.row.medicalDrugExtension.socialSecurityPrice
            ) {
              return (
                <span style="color:red;">
                  {params.row.medicalDrugExtension.retailPrice || 0}元
                </span>
              );
            } else {
              return (
                <span>
                  {params.row.medicalDrugExtension.retailPrice || 0}元
                </span>
              );
            }
          },
        },
        {
          title: "社保指导价",
          key: "medicalDrugExtension.socialSecurityPrice",
          width: 80,
          render: (h, params) => {
            return (
              <span>
                {params.row.medicalDrugExtension.socialSecurityPrice || 0}元
              </span>
            );
          },
        },
        {
          title: "费用类别",
          key: "feeTypeId",
          align: "center",
          width: 80,
          render: (h, params) => {
            let type = this.FeeTypeId.filter((item) => {
              return item.id === params.row.feeTypeId;
            });
            return <span>{type.length ? type[0].name : "无"}</span>;
          },
        },
        // {
        //   title:'浓度',
        //   key:'concentration',
        //   width: 80,
        // },
        {
          title: "生产厂家",
          key: "manufacturer",
          tooltip: true,
        },
        {
          title: "状态",
          key: "dataState",
          width: 80,
          render: (h, params) => {
            if (params.row.dataState === 1) {
              return <tag color="success">可用</tag>;
            } else {
              return <tag color="error">禁用</tag>;
            }
          },
        },
        {
          title: "采购状态",
          key: "applyState",
          width: 80,
          render: (h, params) => {
            if (params.row.applyState === 1) {
              return (
                <i-button
                  type="success"
                  size="small"
                  onClick={() => this.applyStateToggle(params.row)}
                >
                  可采购
                </i-button>
              );
            } else {
              return (
                <i-button
                  type="error"
                  size="small"
                  onClick={() => this.applyStateToggle(params.row)}
                >
                  禁止采购
                </i-button>
              );
            }
          },
        },
        {
          title: "操作",
          key: "action",
          slot: "action",
          width: 80,
          align: "center",
          fixed: "right",
        },
      ],
      formValidate: {
        mayCharges: 1, //是否可划价 默认是：1
        isSellFree: false, //售价可为0 默认为否
        prioritySupplier: "", //优先出库供应商id
        approvalNum: "", //批准文号
        medicalThan: "", // 件比
        yearDosage: 0,
        wareHouseId: [],
        medicalItemName: "",
        medicalItemWorkCode: "",
        brand: "",
        goodsName: "",
        aliasName: "",
        isSplited: 1,
        dataState: 1,
        minDose: "1", // 规格
        packageUnit: "", // 包装单位
        packaging: "1", // 包装规格
        mnemonic: "",
        doseMin: 1,
        monthDosage: 0,
        specificationsQuantity: 1,
        artificialLoss: 0,
        minInventory: 0,
        remark: "",
        specifications: "",
        forms: [],
        // packageSpecifications: 1,
        medicalDrugExtension: {
          purchasingPrice: 0,
          agreementPrice: 0,
          retailPrice: 0,
          referencePrice: 0,
          gocGuidePrice: 0,
          socialSecurityPrice: 0,
        },
        englishName: "",
        doseUnit: "",
        feeTypeId: "",
        iindications: "",
        usage: "",
        storageConditions: "",
        firstStock: "",
        isSalesReturn: 1, //默认可退货
        manufacturer: "",
        concentration: "",
      },
      ruleValidate: {
        wareHouseId: [
          {
            required: true,
            message: "库房类别不能为空",
            trigger: "blur",
            type: "array",
          },
          {
            message: "库房类别不能为空",
            trigger: "change",
            type: "array",
          },
        ],
        medicalItemName: [
          {
            required: true,
            message: "药品名称不能为空",
            trigger: "blur",
          },
        ],
        minDose: [
          {
            required: true,
            message: "规格不能为空",
            trigger: "blur",
          },
        ],
        monthDosage: [
          {
            required: true,
            type: "number",
            message: "患者自然月可用量不能为空",
            trigger: "blur",
          },
        ],
        yearDosage: [
          {
            required: true,
            type: "number",
            message: "自然年最大使用量不能为空",
            trigger: "blur",
          },
        ],
        artificialLoss: [
          {
            required: true,
            type: "number",
            message: "自然月人为损耗量不能为空",
            trigger: "blur",
          },
        ],
        specifications: [
          {
            required: true,
            message: "规格单位不能为空",
            trigger: "blur",
          },
          {
            message: "请选择规格单位",
            trigger: "change",
          },
        ],
        doseUnit: [
          {
            required: true,
            message: "剂量单位不能为空",
            trigger: "blur",
          },
          {
            message: "请选择剂量单位",
            trigger: "change",
          },
        ],
        doseMin: [
          {
            required: true,
            type: "number",
            message: "最小剂量不能为空",
            trigger: "blur",
          },
        ],
        minInventory: [
          {
            required: true,
            type: "number",
            message: "警戒库存不能为空",
            trigger: "blur",
          },
        ],
        specificationsQuantity: [
          {
            required: true,
            type: "number",
            message: "拆分规则必须为有效整数",
            trigger: "blur",
          },
        ],
        forms: [
          {
            required: true,
            message: "剂型不能为空",
            trigger: "blur",
            type: "array",
          },
          {
            message: "剂型不能为空",
            trigger: "change",
            type: "array",
          },
        ],
        packageUnit: [
          {
            required: true,
            message: "包装单位不能为空",
            trigger: "blur",
          },
          {
            message: "包装单位不能为空",
            trigger: "change",
          },
        ],
        packaging: [
          {
            required: true,
            message: "包装规格不能为空",
            trigger: "blur",
          },
          {
            message: "包装规格不能为空",
            trigger: "change",
          },
        ],
        isSplited: [
          {
            required: true,
            message: "请选择是否能拆分",
            trigger: "blur",
            type: "number",
          },
        ],
        mayCharges: [
          {
            required: true,
            message: "请选择是否可划价",
            trigger: "blur",
            type: "number",
          },
        ],
        isSellFree: [
          {
            required: true,
            message: "请选择是否可划价",
            trigger: "blur",
            type: "boolean",
          },
        ],
        isSalesReturn: [
          {
            required: true,
            message: "请选择是否可退货",
            trigger: "blur",
            type: "number",
          },
          {
            message: "请选择是否可退货",
            trigger: "change",
            type: "number",
          },
        ],
        feeTypeId: [
          {
            required: true,
            message: "费用类别不能为空",
            trigger: "blur",
          },
          {
            message: "费用类别不能为空",
            trigger: "change",
          },
        ],
        manufacturer: [
          {
            required: true,
            message: "生产厂家不能为空",
            trigger: "blur",
          },
        ],
        dataState: [
          {
            required: true,
            message: "数据状态不能为空",
            trigger: "blur",
            type: "number",
          },
        ],
      },

      lock: false,
      editLock: false,
      title: "新增",
      TYPENAME: "药品",
      detailTitle: this.TYPENAME,
      cascaderData: [],
      PackageUnit: [], // 包装单位
      Specifications: [], // 规格单位
      SupplierId: [],
      Form: [], // 剂型
      DoseUnit: [], // 剂量单位
      FeeTypeId: [], // 费用类别
      itemId: -1, // 物品id

      // 库房类型id
      WAREHOUSEID: WAREHOUSEID,

      copiedFormValidate: {},
    };
  },
  created() {
    this.copiedFormValidate = deepClone(this.formValidate);
  },
  mounted() {
    this.$nextTick(async () => {
      await this.getDicList();
      await this.$refs["item-file-template"].getItemList(1);
    });
  },
  computed: {},
  mixins: [mixins],
  methods: {
    applyStateToggle(row) {
      let state = row.applyState === 1 ? 2 : 1;
      this.swsApi
        .swsGet(`Data/MedicalItemRecord/ApplyState/${row.id}/${state}`)
        .then((res) => {
          if (res.data.code === 200) {
            this.$Message.success("操作成功！");
            this.$refs["item-file-template"].getItemList();
          } else {
            this.$Message.warning(`操作失败,${res.data.error}`);
          }
        })
        .catch((e) => {
          this.$Message.error(`请求失败,${e}`);
        });
    },
    // 获取数据字典
    getDicList() {
      let apiList = [
        {
          url: "Data/DosageForm/tree",
        },
        {
          url: "Data/MedicalUnit/list",
          params: {
            unitType: 2, // 包装单位
            dataState: 1,
          },
        },
        {
          url: "SystemDictionary/DictionaryList",
          params: {
            typeId: FEETYPEID, // 费用类别
          },
        },
        {
          url: "Data/Supplier/list", // 供应商
        },
        {
          url: "Data/MedicalUnit/list",
          params: {
            unitType: 1, // 规格单位
            dataState: 1,
          },
        },
        {
          url: "Data/MedicalUnit/list",
          params: {
            unitType: 3, // 规格单位
            dataState: 1,
          },
        },
      ];

      return this.swsApi
        .swsAllPost(apiList)
        .then((res) => {
          let treeData = res[0].data.result;
          let reg = new RegExp("title", "g");
          let reg1 = new RegExp("id", "g");
          let cas = JSON.stringify(treeData)
            .replace(reg, "label")
            .replace(reg1, "value");
          let cascaderData = JSON.parse(cas);
          this.Form = cascaderData;
          this.PackageUnit = res[1].data.result;
          this.FeeTypeId = res[2].data.result;
          this.SupplierId = res[3].data.result;
          this.Specifications = res[4].data.result;
          this.DoseUnit = res[5].data.result;
        })
        .catch((e) => {
          console.log(e);
        });
    },
    // 获取物品档案详情
    getItem() {
      this.detailLoading = true;
      this.swsApi
        .swsPost(`Data/MedicalItemRecordView/${this.itemId}`)
        .then((res) => {
          let data = res.data;
          this.detailLoading = false;
          if (!data.error) {
            data.result.medicalDrugExtension =
              data.result.medicalDrugExtension != null
                ? data.result.medicalDrugExtension
                : {
                    purchasingPrice: "",
                    agreementPrice: "",
                    retailPrice: "",
                    referencePrice: "",
                    gocGuidePrice: "",
                    socialSecurityPrice: "",
                  };
            this.formValidate = data.result;
          } else {
            this.$Message.error("网络错误，请稍后再试！");
          }
        })
        .catch(() => {});
    },
    save() {
      this.$refs["formValidate"].validate((valid) => {
        if (valid) {
          let params = Object.assign({}, this.formValidate);
          this.title === "新增" && params.id && delete params.id;
          params.wareHouseId = params.wareHouseId.pop();
          params.forms = params.forms.pop();
          params.medicalItemTypeEnum = 1;
          this.swsApi
            .swsPost("Data/MedicalItemRecord/CreateUpdate", params)
            .then((res) => {
              console.log(res);
              let data = res.data;
              this.loading = false;
              if (data.success) {
                this.addFlag = false;
                this.$Message.success("操作成功");
                this.$refs["item-file-template"].getItemList(
                  this.$refs["item-file-template"].startPage
                );
                this.cancel("formValidate");
              } else {
                this.$Message.error(`操作失败,${data.error}`);
                this.cancel("formValidate");
              }
            })
            .catch((e) => {
              console.log(e);
            });
        } else {
          this.$Message.error("请完善必填信息");
        }
      });
    },
    cascaderChange() {},
  },
  components: {
    itemFileTemplate,
    Operate,
  },
};
</script>

<style scoped lang="less">
#item-file {
  height: 100%;
  background: #ffffff;
  overflow: hidden;
  .aside {
    padding: 20px;
    /deep/ .ivu-card {
      margin-left: 20px;
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
  .button-group {
    button + button {
      margin-left: 10px;
    }
  }

  .table {
    margin-top: 20px;
    padding-right: 20px;
    .ivu-table-wrapper {
      border: none !important;
      & /deep/ .ivu-table {
        &::before,
        &::after {
          display: none !important;
        }
      }
      & /deep/ .ivu-table th {
        font-size: 14px;
        background: #f2f8ff;
        border-bottom: none;
      }
    }
  }
}
/deep/ .ivu-modal-body {
  padding: 0 16px;
}
.red-item-label {
  width: 130px;
  color: red;
  text-align: right;
  vertical-align: middle;
  float: left;
  font-size: 12px;
  line-height: 1;
  padding: 10px 12px 10px 0;
  -webkit-box-sizing: border-box;
  box-sizing: border-box;
}
.red-item-label:before {
  content: "*";
  display: inline-block;
  margin-right: 4px;
  line-height: 1;
  font-family: SimSun;
  font-size: 12px;
  color: #ed4014;
}
.form {
  position: relative;
  .title-row {
    font-size: 14px;
    margin: 10px 0;
    padding-left: 10px;
    border-left: 2px solid #3399ff;
  }
  .basic-row {
    padding: 20px 20px 0 0;
    border-radius: 8px;
    border: 1px solid #d8d8d8;
    margin-bottom: 10px;
  }
}
.w-100 {
  width: 100%;
}
.medMIC {
  position: relative;
  .medMICP {
    position: absolute;
    font-size: 11px;
    color: red;
    top: 25px;
  }
}
</style>
