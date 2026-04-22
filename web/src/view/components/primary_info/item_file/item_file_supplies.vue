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
              <FormItem label="耗材类别" prop="wareHouseId">
                <Cascader :data="cascaderData" v-model="formValidate.wareHouseId" :disabled="readonly" change-on-select @on-change="cascaderChange"></Cascader>
              </FormItem>
              </Col>
              <Col span="12">
              <label class="red-item-label">耗材名称</label>
              <FormItem prop="medicalItemName">
                <Input placeholder="请输入" v-model="formValidate.medicalItemName" :readonly="readonly" :disabled="lock"></Input>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem label="耗材编码" prop="medicalItemWorkCode">
                <Input placeholder="请输入" v-model="formValidate.medicalItemWorkCode" :readonly="readonly"></Input>
              </FormItem>
              </Col>
              <!-- 新增 注册证/备案凭证号 -->
              <Col span="12">
              <FormItem label="注册证/备案凭证号" prop="approvalNum">
                <Input placeholder="请输入" v-model="formValidate.approvalNum" :readonly="readonly"></Input>
              </FormItem>
              </Col>

              <Col span="12">
              <FormItem label="品牌/型号" prop="brand">
                <Input placeholder="请输入" v-model="formValidate.brand" :readonly="readonly"></Input>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem label="英文名称" prop="englishName">
                <Input placeholder="请输入" v-model="formValidate.englishName" :readonly="readonly"></Input>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem label="助记码" prop="mnemonic">
                <Input placeholder="请输入" v-model="formValidate.mnemonic" :readonly="readonly" :disabled="editLock"></Input>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem prop="packageUnit" label="包装单位">
                <Select v-model="formValidate.packageUnit" :disabled="readonly">
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
              <FormItem prop="packaging" label="包装规格">
                <Input placeholder="请输入" v-model="formValidate.packaging" :readonly="readonly"></Input>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem label="规格单位" prop="specifications">
                <Select placeholder="请选择" v-model="formValidate.specifications" :disabled="readonly">
                  <Option :value="item.id" v-for="(item, i) in Specifications" :key="i">{{item.speUnitCHS}}</Option>
                </Select>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem prop="feeTypeId" label="费用类别">
                <Select v-model="formValidate.feeTypeId" :disabled="readonly">
                  <Option :value="item.id" v-for="(item, i) in FeeTypeId" :key="i">{{item.name}}</Option>
                </Select>
              </FormItem>
              </Col>
              <Col span="12">
              <label class="red-item-label">生产厂家</label>
              <FormItem prop="manufacturer">
                <Input placeholder="请输入" v-model="formValidate.manufacturer" :readonly="readonly" :disabled="lock"></Input>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem prop="doseUnit" label="优先出库供应商">
                <Select v-model="formValidate.prioritySupplier" filterable clearable :disabled="readonly">
                  <Option :value="item.id" v-for="(item, i) in SupplierId" :key="i">{{item.name}}</Option>
                </Select>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem prop="medicalThan" label="件比">
                <Input placeholder="请输入" v-model="formValidate.medicalThan" :readonly="readonly"></Input>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem prop="specificationsQuantity" label="包装拆分规则">
                <InputNumber placeholder="请输入" v-model="formValidate.specificationsQuantity" style="width:100%" :readonly="readonly" :disabled="editLock || formValidate.isSplited!==1"></InputNumber>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem label="警戒库存" prop="minInventory">
                <InputNumber placeholder="请输入" v-model="formValidate.minInventory" style="width:100%" :readonly="readonly" :disabled="editLock"></InputNumber>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem prop="isSplited" label="是否可拆分">
                <RadioGroup v-model="formValidate.isSplited">
                  <Radio :label="1" :disabled="editLock">是</Radio>
                  <Radio :label="0" :disabled="editLock">否</Radio>
                </RadioGroup>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem prop="isMedCanal" label="是否为管路">
                <RadioGroup v-model="formValidate.isMedCanal">
                  <Radio :label="1" :disabled="editLock">是</Radio>
                  <Radio :label="0" :disabled="editLock">否</Radio>
                </RadioGroup>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem prop="dialyzerType" label="透析器类型">
                <Select v-model="formValidate.dialyzerType">
                  <Option :value="item.index" v-for="item in dialyzerTypeList" :key="item.index">{{item.name}}</Option>
                </Select>
              </FormItem>
              </Col>
              <!-- 新增  mayCharges  是否可划价-->
              <Col span="12">
              <FormItem prop="mayCharges" label="是否可划价">
                <RadioGroup v-model="formValidate.mayCharges">
                  <Radio :label="1" :disabled="readonly">是</Radio>
                  <Radio :label="0" :disabled="readonly">否</Radio>
                </RadioGroup>
              </FormItem>
              </Col>
            </Row>

          </div>

          <p class="title-row">产品说明</p>
          <div class="basic-row">
            <Row>
              <Col span="12">
              <FormItem label="患者自然月可用量" prop="monthDosage">
                <InputNumber :min="0" class="w-100" placeholder="请输入" v-model="formValidate.monthDosage" :readonly="readonly" :disabled="editLock"></InputNumber>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem label="自然月人为损耗量" prop="artificialLoss">
                <InputNumber :min="0" class="w-100" placeholder="请输入" v-model="formValidate.artificialLoss" :readonly="readonly" :disabled="editLock"></InputNumber>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem label="自然年最大使用量" prop="yearDosage">
                <InputNumber :min="0" class="w-100" :readonly="readonly" placeholder="请输入" v-model="formValidate.yearDosage" :disabled="editLock"></InputNumber>
              </FormItem>
              </Col>
              <Col span="24">
              <FormItem label="用途" prop="iindications">
                <Input placeholder="请输入" type="textarea" v-model="formValidate.iindications" :readonly="readonly"></Input>
              </FormItem>
              </Col>
              <Col span="24">
              <FormItem label="使用说明" prop="usage">
                <Input placeholder="请输入" type="textarea" v-model="formValidate.usage" :readonly="readonly"></Input>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem label="存储条件" prop="storageConditions">
                <Input placeholder="请输入" v-model="formValidate.storageConditions" :readonly="readonly"></Input>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem label="首批进货人员" prop="firstStock">
                <Input placeholder="请输入" v-model="formValidate.firstStock" :readonly="readonly"></Input>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem prop="dataState" label="数据状态">
                <RadioGroup v-model="formValidate.dataState">
                  <Radio :label="1" :disabled="editLock || readonly">可用</Radio>
                  <Radio :label="2" :disabled="editLock || readonly">停用</Radio>
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
              <FormItem label="建议采购价" prop="medicalDrugExtension.purchasingPrice">
                <InputNumber :min="0" class="w-100" placeholder="请输入" v-model="formValidate.medicalDrugExtension.purchasingPrice" :readonly="readonly" :disabled="editLock" @on-blur="() => this.setDefaultPrice('purchasingPrice')"></InputNumber>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem label="零售价" prop="medicalDrugExtension.retailPrice">
                <InputNumber :min="0" class="w-100" placeholder="请输入" v-model="formValidate.medicalDrugExtension.retailPrice" :readonly="readonly" :disabled="editLock" @on-blur="() => this.setDefaultPrice('retailPrice')"></InputNumber>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem label="协议价" prop="medicalDrugExtension.agreementPrice">
                <InputNumber :min="0" class="w-100" placeholder="请输入" v-model="formValidate.medicalDrugExtension.agreementPrice" :readonly="readonly" :disabled="editLock" @on-blur="() => this.setDefaultPrice('agreementPrice')"></InputNumber>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem label="参考价" prop="medicalDrugExtension.referencePrice">
                <InputNumber :min="0" class="w-100" placeholder="请输入" v-model="formValidate.medicalDrugExtension.referencePrice" :readonly="readonly" :disabled="editLock" @on-blur="() => this.setDefaultPrice('referencePrice')"></InputNumber>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem label="政府指导价" prop="medicalDrugExtension.gocGuidePrice">
                <InputNumber :min="0" class="w-100" placeholder="请输入" v-model="formValidate.medicalDrugExtension.gocGuidePrice" :readonly="readonly" :disabled="editLock" @on-blur="() => this.setDefaultPrice('gocGuidePrice')"></InputNumber>
              </FormItem>
              </Col>
              <Col span="12">
              <FormItem label="社保指导价" prop="medicalDrugExtension.socialSecurityPrice">
                <InputNumber :min="0" class="w-100" placeholder="请输入" v-model="formValidate.medicalDrugExtension.socialSecurityPrice" :readonly="readonly" :disabled="editLock" @on-blur="(value) => this.setDefaultPrice('socialSecurityPrice')"></InputNumber>
              </FormItem>
              </Col>
              <Col span="24">
              <FormItem label="备注" prop="remark">
                <Input placeholder="请输入" type="textarea" v-model="formValidate.remark" :readonly="readonly" :disabled="editLock"></Input>
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
import mixins from "./mixins.js";
import { deepClone } from "@/libs/tools.js";
import Operate from "@/components/operate";
const FEETYPEID = "31166b331f7b2596974b6977b9fd5ac2";
const WAREHOUSEID = 2;
const IMPORTID = 5;
export default {
  name: "item_file_supplies",
  data() {
    return {
      dialyzerTypeList: [
        { index: 0, name: "无" },
        { index: 1, name: "低通" },
        { index: 2, name: "高通" },
        { index: 3, name: "血滤" },
      ],
      importId: IMPORTID,
      treeData: [],
      wareHouseId: -1, // 库房id
      loading: false,
      detailLoading: false,
      item_data: [],
      addFlag: false,
      editPriceFlag: false,
      table_column: [
        {
          title: "名称",
          fixed: "left",
          key: "medicalItemName",
          minWidth: 100,
          // maxWidth: 180,
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
          minWidth: 80,
        },
        {
          title: "编码",
          key: "medicalItemCode",
          minWidth: 100,
        },
        {
          title: "助记码",
          key: "mnemonic",
          minWidth: 100,
          // maxWidth: 180
        },
        {
          title: "规格",
          key: "minDose",
          minWidth: 100,
          // maxWidth: 180
        },
        {
          title: "包装",
          key: "packaging",
          width: 80,
        },
        {
          title: "建议采购价",
          key: "medicalDrugExtension.purchasingPrice",
          align: "center",
          minWidth: 75,
          maxWidth: 120,
          render: (h, params) => {
            return (
              <span>
                {params.row.medicalDrugExtension.purchasingPrice || 0}元
              </span>
            );
          },
        },
        {
          title: "建议销售价",
          key: "medicalDrugExtension.retailPrice",
          align: "center",
          minWidth: 75,
          maxWidth: 120,
          render: (h, params) => {
            return (
              <span>{params.row.medicalDrugExtension.retailPrice || 0}元</span>
            );
          },
        },
        {
          title: "费用类别",
          key: "feeTypeId",
          align: "center",
          minWidth: 75,
          maxWidth: 120,
          render: (h, params) => {
            let type = this.FeeTypeId.filter((item) => {
              return item.id === params.row.feeTypeId;
            })[0].name;
            return <span>{type}</span>;
          },
        },
        {
          title: "生产厂家",
          key: "manufacturer",
          tooltip: true,
          minWidth: 100,
        },
        {
          title: "状态",
          key: "dataState",
          align: "center",
          minWidth: 75,
          maxWidth: 120,
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
                  // this.showEdit(params.row)
                  // cy 调用item-template模板里的方法
                  this.$refs["item-file-template"].showEditPrice(params.row);
                  delete this.formValidate.hiCenterCode;
                  delete this.formValidate.form;
                  delete this.formValidate.forms;
                  delete this.formValidate.aliasName;
                  delete this.formValidate.goodsName;
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
          fixed: "left",
          key: "medicalItemName",
          minWidth: 100,
          // maxWidth: 180,
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
          minWidth: 80,
        },
        {
          title: "编码",
          key: "medicalItemCode",
          minWidth: 100,
        },
        {
          title: "助记码",
          key: "mnemonic",
          minWidth: 100,
          // maxWidth: 180
        },
        {
          title: "规格",
          key: "minDose",
          minWidth: 100,
          // maxWidth: 180
        },
        {
          title: "包装",
          key: "packaging",
          width: 80,
        },
        {
          title: "建议采购价",
          key: "medicalDrugExtension.purchasingPrice",
          align: "center",
          minWidth: 75,
          maxWidth: 120,
          render: (h, params) => {
            return (
              <span>
                {params.row.medicalDrugExtension.purchasingPrice || 0}元
              </span>
            );
          },
        },
        {
          title: "建议销售价",
          key: "medicalDrugExtension.retailPrice",
          align: "center",
          minWidth: 75,
          maxWidth: 120,
          render: (h, params) => {
            return (
              <span>{params.row.medicalDrugExtension.retailPrice || 0}元</span>
            );
          },
        },
        {
          title: "费用类别",
          key: "feeTypeId",
          align: "center",
          minWidth: 75,
          maxWidth: 120,
          render: (h, params) => {
            let type = this.FeeTypeId.filter((item) => {
              return item.id === params.row.feeTypeId;
            })[0].name;
            return <span>{type}</span>;
          },
        },
        {
          title: "生产厂家",
          key: "manufacturer",
          tooltip: true,
          minWidth: 100,
        },
        {
          title: "状态",
          key: "dataState",
          align: "center",
          minWidth: 75,
          maxWidth: 120,
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
        approvalNum: "", //注册证/备案凭证号
        mayCharges: 1, //是否可划价 有价卫材1  普通卫材0
        prioritySupplier: "", //优先出库供应商id
        medicalThan: "",
        yearDosage: 0,
        wareHouseId: [],
        medicalItemName: "",
        medicalItemWorkCode: "",
        isSplited: 1,
        isMedCanal: 0,
        dialyzerType: 0,
        dataState: 1,
        minDose: "1", // 规格
        packageUnit: "", // 包装单位
        packaging: "1", // 包装规格
        mnemonic: "",
        brand: "",
        englishName: "",
        feeTypeId: "",
        iindications: "",
        usage: "",
        specificationsQuantity: 1,
        minInventory: 1,
        monthDosage: 0,
        artificialLoss: 0,
        // packageSpecifications: 1,
        remark: "",
        specifications: "",
        medicalDrugExtension: {
          purchasingPrice: 0,
          agreementPrice: 0,
          retailPrice: 0,
          referencePrice: 0,
          gocGuidePrice: 0,
          socialSecurityPrice: 0,
        },
        storageConditions: "",
        firstStock: "",
        isSalesReturn: 1, //默认可退货
        manufacturer: "",
      },
      ruleValidate: {
        mayCharges: [
          {
            required: true,
            message: "请选择是否可划价",
            trigger: "blur",
            type: "number",
          },
        ],
        wareHouseId: [
          {
            required: true,
            message: "耗材类别不能为空",
            trigger: "blur",
            type: "array",
          },
          {
            message: "耗材类别不能为空",
            trigger: "change",
            type: "array",
          },
        ],
        medicalItemName: [
          {
            required: true,
            message: "耗材名称不能为空",
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
        minInventory: [
          {
            required: true,
            message: "最小库存不能为空",
            trigger: "blur",
            type: "number",
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
          // {
          //   message: '费用类别不能为空',
          //   trigger: 'change'
          // }
        ],
        // supplierId: [{
        //   required: true,
        //   message: '供货商不能为空',
        //   trigger: 'blur',
        // },{
        //   message: '供货商不能为空',
        //   trigger: 'change',
        // }],
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
        specificationsQuantity: [
          {
            required: true,
            type: "number",
            message: "拆分规则必须为有效整数",
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
        artificialLoss: [
          {
            required: true,
            type: "number",
            message: "自然月人为损耗量不能为空",
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
      },

      lock: false,
      editLock: false,
      title: "新增",
      TYPENAME: "卫生耗材",
      detailTitle: this.TYPENAME,
      cascaderData: [],
      PackageUnit: [], // 包装单位
      Specifications: [], // 规格单位
      SupplierId: [],
      FeeTypeId: [],
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
    this.$nextTick(() => {
      this.getDicList().then(() => {
        this.$refs["item-file-template"].getItemList(1);
      });
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
      ];

      return this.swsApi
        .swsAllPost(apiList)
        .then((res) => {
          this.PackageUnit = res[0].data.result;
          this.FeeTypeId = res[1].data.result;
          this.SupplierId = res[2].data.result;
          this.Specifications = res[3].data.result;
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
        });
    },
    save() {
      this.$refs["formValidate"].validate((valid) => {
        if (valid) {
          this.addFlag = false;
          let params = Object.assign({}, this.formValidate);
          this.title === "新增" && params.id && delete params.id;
          params.wareHouseId = params.wareHouseId.pop();
          params.medicalItemTypeEnum = 2;
          // cy：删除该参数
          params.forms && delete params.forms;
          this.swsApi
            .swsPost("Data/MedicalItemRecord/CreateUpdate", params)
            .then((res) => {
              let data = res.data;
              this.loading = false;
              let child = this.$refs["item-file-template"];
              if (data.success) {
                this.$Message.success("操作成功");
                child.getItemList(child.startPage);
                this.cancel("formValidate");
              } else {
                this.$Message.error(`操作失败,${data.error}`);
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
    cascaderChange(v, k) {
      if (v[0] === "187b752e52414196b763f91fae53a825") {
        console.log("有价卫材");
        this.formValidate.mayCharges = 1;
      } else {
        console.log("普通卫材");
        this.formValidate.mayCharges = 0;
      }
      // this.wareHouseId = v.pop()
    },
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
</style>
