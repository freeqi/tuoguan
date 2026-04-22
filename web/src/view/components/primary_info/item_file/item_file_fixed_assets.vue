<template>
  <div id="item-file">
    <item-file-template ref="item-file-template" :table-column="table_column" :table-column2="table_column2" :wareh-house-type-id="WAREHOUSEID" @show-add-modal="showAdd" @get-cascader="getCascader" @export-tb="exportTb" :importId="importId"></item-file-template>

    <Modal v-model="addFlag" width="800" @on-cancel="cancel('formValidate')" :mask-closable="false">
      <p slot="header" style="text-align: center;">{{title}}{{detailTitle}}档案</p>
      <Form ref="formValidate" class="form" :model="formValidate" :label-width="130" :rules="ruleValidate">
        <div>
          <Row>
            <Col span="24">
            <FormItem label="固定资产类别" prop="wareHouseId">
              <Cascader :data="cascaderData" v-model="formValidate.wareHouseId" :disabled="readonly" change-on-select @on-change="cascaderChange"></Cascader>
            </FormItem>
            </Col>
            <Col span="24">
            <FormItem label="名称" prop="medicalItemName">
              <Input placeholder="请输入" v-model="formValidate.medicalItemName" :readonly="readonly" :disabled="lock"></Input>
            </FormItem>
            </Col>
            <Col span="12">
            <FormItem label="固定资产编码" prop="medicalItemWorkCode">
              <Input placeholder="请输入" v-model="formValidate.medicalItemWorkCode" :readonly="readonly"></Input>
            </FormItem>
            </Col>
            <Col span="12">
            <FormItem label="助记码" prop="mnemonic">
              <Input placeholder="请输入" v-model="formValidate.mnemonic" :readonly="readonly" :disabled="editLock"></Input>
            </FormItem>
            </Col>
            <Col span="12">
            <FormItem label="品牌/型号" prop="brand">
              <Input placeholder="请输入" v-model="formValidate.brand" :readonly="readonly"></Input>
            </FormItem>
            </Col>
            <Col span="12">
            <FormItem prop="packageUnit" label="包装单位">
              <Select v-model="formValidate.packageUnit" :disabled="lock || readonly">
                <Option :value="item.id" v-for="(item, i) in PackageUnit" :key="i">{{item.speUnitCHS}}</Option>
              </Select>
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
            <Col span="12">
            <FormItem prop="dataState" label="数据状态">
              <RadioGroup v-model="formValidate.dataState">
                <Radio :label="1" :disabled="editLock || readonly">可用</Radio>
                <Radio :label="2" :disabled="editLock || readonly">停用</Radio>
              </RadioGroup>
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
          </Row>
          <Row>
            <Col span="12">
            <FormItem prop="manufacturer" label="生产厂家">
              <Input placeholder="请输入" v-model="formValidate.manufacturer" :readonly="readonly" :disabled="lock"></Input>
            </FormItem>
            </Col>
            <Col span="12">
            <FormItem prop="doseUnit" label="优先出库供应商">
              <Select v-model="formValidate.prioritySupplier" filterable>
                <Option :value="item.id" v-for="(item, i) in SupplierId" :key="i">{{item.name}}</Option>
              </Select>
            </FormItem>
            </Col>
            <Col span="12">
            <FormItem prop="hasee" label="售后电话">
              <Input placeholder="请输入" v-model="formValidate.hasee" :readonly="readonly"></Input>
            </FormItem>
            </Col>
            <Col span="12">
            <FormItem label="建议采购价" prop="medicalDrugExtension.purchasingPrice">
              <InputNumber :min="0" class="w-100" placeholder="请输入" v-model="formValidate.medicalDrugExtension.purchasingPrice" :readonly="readonly" :disabled="editLock" @on-blur="() => this.setDefaultPrice('purchasingPrice')"></InputNumber>
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
            <Col span="24">
            <FormItem label="用途" prop="iindications">
              <Input placeholder="请输入" type="textarea" v-model="formValidate.iindications" :readonly="readonly" :disabled="editLock"></Input>
            </FormItem>
            </Col>
            <Col span="24">
            <FormItem label="备注" prop="remark">
              <Input placeholder="请输入" type="textarea" v-model="formValidate.remark" :readonly="readonly" :disabled="editLock"></Input>
            </FormItem>
            </Col>
          </Row>
        </div>
        <Spin size="small" fix v-if="detailLoading"></Spin>
      </Form>
      <div slot="footer">
        <Button type="info" v-show="title === '新增'" @click="stickValue">黏贴</Button>
        <Button type="info" v-show="title === '查看' || title === '修改'" @click="copyValue">复制</Button>
        <Button type="primary" @click="save" v-show="saveModal">保存</Button>
        <Button type="default" @click="cancel('formValidate')">取消</Button>
      </div>
    </Modal>
  </div>
</template>

<script>
import itemFileTemplate from "@/components/item-file-template";
import Operate from "@/components/operate";
import mixins from "./mixins.js";
import { deepClone } from "@/libs/tools.js";
const WAREHOUSEID = 3;
const IMPORTID = 6;
export default {
  name: "fixed_assets",
  data() {
    return {
      importId: IMPORTID,
      treeData: [],
      wareHouseId: -1, // 库房id
      loading: false,
      detailLoading: false,
      item_data: [],
      addFlag: false,
      table_column: [
        {
          title: "名称",
          key: "medicalItemName",
          width: 300,
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
        },
        {
          title: "建议采购价",
          key: "medicalDrugExtension.purchasingPrice",
          // width: 120,
          align: "center",
          render: (h, params) => {
            return (
              <span>
                {params.row.medicalDrugExtension.purchasingPrice || 0}元
              </span>
            );
          },
        },
        // {
        //   title: '供货商',
        //   key: 'supplierName',
        //   tooltip: true
        // },
        {
          title: "状态",
          key: "dataState",
          align: "center",
          // width: 120,
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
                handleDelete={() => this.showDel(params.row)}
                showDelete={params.row.medInventory <= 0}
                handleEdit={() => {
                  this.showEdit(params.row);
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
          key: "medicalItemName",
          width: 300,
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
        },
        {
          title: "建议采购价",
          key: "medicalDrugExtension.purchasingPrice",
          // width: 120,
          align: "center",
          render: (h, params) => {
            return (
              <span>
                {params.row.medicalDrugExtension.purchasingPrice || 0}元
              </span>
            );
          },
        },
        // {
        //   title: '供货商',
        //   key: 'supplierName',
        //   tooltip: true
        // },
        {
          title: "状态",
          key: "dataState",
          align: "center",
          // width: 120,
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
        mayCharges: 0, //是否可划价 默认否：0
        prioritySupplier: "", //优先出库供应商id
        wareHouseId: [],
        medicalItemName: "",
        medicalItemWorkCode: "",
        brand: "",
        isSalesReturn: 1, //默认可退货
        dataState: 1,
        packageUnit: "", // 包装单位
        hasee: "", // 售后电话
        mnemonic: "",
        remark: "",
        manufacturer: "",
        iindications: "",
        medicalDrugExtension: {
          purchasingPrice: 0,
          agreementPrice: 0,
          referencePrice: 0,
        },
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
      },

      lock: false,
      editLock: false,
      title: "新增",
      TYPENAME: "固定资产",
      detailTitle: this.TYPENAME,
      cascaderData: [],
      PackageUnit: [], // 包装单位
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
      this.getDicList();
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
          url: "Data/Supplier/list", // 供应商
        },
      ];
      this.swsApi
        .swsAllPost(apiList)
        .then((res) => {
          this.PackageUnit = res[0].data.result;
          this.SupplierId = res[1].data.result;
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
                    referencePrice: "",
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
          params.medicalItemTypeEnum = 3;
          this.swsApi
            .swsPost("Data/MedicalItemRecord/CreateUpdate", params)
            .then((res) => {
              let data = res.data;
              this.loading = false;
              if (data.success) {
                this.$Message.success("操作成功");
                this.$refs["item-file-template"].getItemList(
                  this.$refs["item-file-template"].startPage
                );
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
    cascaderChange(v) {
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
.form {
  position: relative;
}
.w-100 {
  width: 100%;
}
</style>
