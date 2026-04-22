<template>
  <div id="item-file">
    <item-file-template
      ref="item-file-template"
      :table-column="table_column"
      :wareh-house-type-id="WAREHOUSEID"
      @show-add-modal="showAdd"
      @get-cascader="getCascader"
      @export-tb="exportTb"
      :importId="importId"
      :stopAutoLoad="true"
    ></item-file-template>

    <Modal v-model="addFlag" width="800" @on-cancel="cancel('formValidate')" :mask-closable="false">
      <p slot="header" style="text-align: center;">{{title}}{{detailTitle}}档案</p>
      <Form
        ref="formValidate"
        class="form"
        :model="formValidate"
        :label-width="130"
        :rules="ruleValidate"
      >
        <div>
          <Row>
            <Col span="24">
              <FormItem label="诊疗类别" prop="wareHouseId">
                <Cascader
                  :data="cascaderData"
                  v-model="formValidate.wareHouseId"
                  :disabled="readonly"
                  change-on-select
                  @on-change="cascaderChange"
                ></Cascader>
              </FormItem>
            </Col>
            <Col span="12">
              <FormItem label="项目名称" prop="medicalItemName">
                <Input
                  placeholder="请输入"
                  v-model="formValidate.medicalItemName"
                  :readonly="readonly"
                ></Input>
              </FormItem>
            </Col>
            <Col span="12">
              <FormItem label="项目编码" prop="medicalItemWorkCode">
                <Input
                  placeholder="请输入"
                  v-model="formValidate.medicalItemWorkCode"
                  :readonly="readonly"
                ></Input>
              </FormItem>
            </Col>
            <Col span="12">
              <FormItem label="助记码" prop="mnemonic">
                <Input
                  placeholder="请输入"
                  v-model="formValidate.mnemonic"
                  :readonly="readonly"
                  :disabled="editLock"
                ></Input>
              </FormItem>
            </Col>
            <Col span="12">
              <FormItem label="单位" prop="specifications">
                <Select
                  placeholder="请选择"
                  v-model="formValidate.specifications"
                  :disabled="readonly"
                >
                  <Option
                    :value="item.id"
                    v-for="(item, i) in Specifications"
                    :key="i"
                  >{{item.speUnitCHS}}</Option>
                </Select>
              </FormItem>
            </Col>
            <Col span="12">
              <FormItem label="规格" prop="minDose">
                <Input placeholder="请输入" v-model="formValidate.minDose" :readonly="readonly"></Input>
              </FormItem>
            </Col>
            <Col span="12">
              <FormItem label="患者单次可用次数" prop="monthDosage">
                <InputNumber
                  :min="0"
                  class="w-100"
                  placeholder="请输入"
                  v-model="formValidate.monthDosage"
                  :readonly="readonly"
                  :disabled="editLock"
                ></InputNumber>
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
              <FormItem label="单价" prop="medicalDrugExtension.retailPrice">
                <InputNumber
                  :min="0"
                  class="w-100"
                  placeholder="请输入"
                  v-model="formValidate.medicalDrugExtension.retailPrice"
                  :readonly="readonly"
                  :disabled="editLock"
                  @on-blur="() => this.setDefaultPrice('retailPrice')"
                ></InputNumber>
              </FormItem>
            </Col>
            <Col span="12">
              <FormItem label="建议采购价" prop="medicalDrugExtension.purchasingPrice">
                <InputNumber
                  :min="0"
                  class="w-100"
                  placeholder="请输入"
                  v-model="formValidate.medicalDrugExtension.purchasingPrice"
                  :readonly="readonly"
                  :disabled="editLock"
                  @on-blur="() => this.setDefaultPrice('purchasingPrice')"
                ></InputNumber>
              </FormItem>
            </Col>
            <Col span="12">
              <FormItem label="参考价" prop="medicalDrugExtension.referencePrice">
                <InputNumber
                  :min="0"
                  class="w-100"
                  placeholder="请输入"
                  v-model="formValidate.medicalDrugExtension.referencePrice"
                  :readonly="readonly"
                  :disabled="editLock"
                  @on-blur="() => this.setDefaultPrice('referencePrice')"
                ></InputNumber>
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
            <!-- 新增  mayCharges  是否可划价-->
              <Col span="12">
                <FormItem prop="mayCharges" label="是否可划价">
                  <RadioGroup v-model="formValidate.mayCharges">
                    <Radio :label="1" :disabled="readonly">是</Radio>
                    <Radio :label="0" :disabled="readonly">否</Radio>
                  </RadioGroup>
                </FormItem>
              </Col>

            <Col span="24">
              <FormItem label="内容" prop="usage">
                <Input
                  placeholder="请输入"
                  type="textarea"
                  v-model="formValidate.usage"
                  :readonly="readonly"
                ></Input>
              </FormItem>
            </Col>
            <Col span="24">
              <FormItem label="备注" prop="remark">
                <Input
                  placeholder="请输入"
                  type="textarea"
                  v-model="formValidate.remark"
                  :readonly="readonly"
                  :disabled="editLock"
                ></Input>
              </FormItem>
            </Col>
          </Row>
        </div>
        <Spin size="small" fix v-if="detailLoading"></Spin>
      </Form>
      <div slot="footer">
        <Button
          type="success"
          @click="showEdit(formValidate)"
          v-show="!saveModal"
          v-permission="buttonRole.WPDA_BJ"
        >编辑</Button>
        <Button type="primary" @click="save" v-show="saveModal">保存</Button>
        <Button type="default" @click="cancel('formValidate')">取消</Button>
      </div>
    </Modal>
  </div>
</template>

<script>
import itemFileTemplate from '@/components/item-file-template'
import mixins from './mixins.js'
import Operate from '@/components/operate'
const FEETYPEID = '31166b331f7b2596974b6977b9fd5ac2'
const WAREHOUSEID = 5
const IMPORTID = 8
export default {
  name: 'diagnostic_programs',
  data () {
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
          title: '名称',
          key: 'medicalItemName',
          width: 180
        },
        {
          title: '编码',
          key: 'medicalItemCode',
        },
        {
          title: '助记码',
          key: 'mnemonic',
          width: 120
        },
        {
          title: '规格',
          key: 'minDose',
          align: 'center'
        },
        {
          title: '费用类别',
          key: 'feeTypeId',
          align: 'center',
          render: (h, params) => {
            let type = this.FeeTypeId.filter(item => {
              return item.id === params.row.feeTypeId
            })[0].name
            return <span>{type}</span>
          }
        },
        {
          title: '建议采购价',
          key: 'medicalDrugExtension.purchasingPrice',
          align: 'center',
          render: (h, params) => {
            if (params.row.medicalDrugExtension.centerName === '统一价') {
              return (<span style="color:blue;">{params.row.medicalDrugExtension.purchasingPrice || 0}元</span>)
            } else {
              return (<span>{params.row.medicalDrugExtension.purchasingPrice || 0}元</span>)
            }
          }
        },
        {
          title: '建议销售价',
          key: 'medicalDrugExtension.retailPrice',
          align: 'center',
          render: (h, params) => {
            if (params.row.medicalDrugExtension.socialSecurityPrice !== 0 && params.row.medicalDrugExtension.retailPrice > params.row.medicalDrugExtension.socialSecurityPrice) {
              return (
                <span style="color:red;">{params.row.medicalDrugExtension.retailPrice || 0}元</span>
              )
            } else {
              return (
                <span>{params.row.medicalDrugExtension.retailPrice || 0}元</span>
              )
            }
          }
        },
        {
          title: '社保指导价',
          key: 'medicalDrugExtension.socialSecurityPrice',
          render: (h, params) => {
            return (
              <span>{params.row.medicalDrugExtension.socialSecurityPrice || 0}元</span>
            )
          }
        },
        {
          title: '状态',
          key: 'dataState',
          align: 'center',
          render: (h, params) => {
            if (params.row.dataState === 1) {
              return <tag color="success">可用</tag>
            } else {
              return <tag color="error">禁用</tag>
            }
          }
        },
        {
          title: '操作',
          key: 'action',
          width: 120,
          align: 'center',
          fixed: 'right',
          render: (h, params) => {
            return (
              <Operate
                textEdit="调价"
                handleDelete={() => this.showDel(params.row)}
                handleEdit={() => {
                  // this.showEdit(params.row)
                  // cy 调用item-template模板里的方法
                  this.$refs['item-file-template'].showEditPrice(params.row)
                  delete this.formValidate.hiCenterCode
                  delete this.formValidate.form
                }}
                handleWatch={() =>
                  this.showDetail(params.row.id, params.row.medicalItemName)
                }
                permissionEdit={this.buttonRole.WPDA_XG}
                permissionDelete={this.buttonRole.WPDA_SC}
              />
            )
          }
        }
      ],
      formValidate: {
        mayCharges:1, //是否可划价 默认是：1
        wareHouseId: [],
        medicalItemName: '',
        medicalItemWorkCode: '',
        dataState: 1,
        mnemonic: '',
        remark: '',
        usage: '',
        minDose: '',
        specifications: '',
        monthDosage: 0,
        medicalDrugExtension: {
          purchasingPrice: 0,
          retailPrice: 0,
          referencePrice: 0
        }
      },
      ruleValidate: {
        mayCharges: [
          {
            required: true,
            message: '请选择是否可划价',
            trigger: 'blur',
            type: 'number'
          }
        ],
        wareHouseId: [
          {
            required: true,
            message: '诊疗类别不能为空',
            trigger: 'blur',
            type: 'array'
          },
          {
            message: '诊疗类别不能为空',
            trigger: 'change',
            type: 'array'
          }
        ],
        medicalItemName: [
          {
            required: true,
            message: '项目名称不能为空',
            trigger: 'blur'
          }
        ],
        minDose: [
          {
            required: true,
            message: '规格不能为空',
            trigger: 'blur'
          }
        ],
        specifications: [
          {
            required: true,
            message: '规格单位不能为空',
            trigger: 'blur'
          },
          {
            message: '请选择规格单位',
            trigger: 'blur'
          }
        ],
        feeTypeId: [
          {
            required: true,
            message: '费用类别不能为空',
            trigger: 'blur'
          },
          {
            message: '费用类别不能为空',
            trigger: 'change'
          }
        ],
        dataState: [
          {
            required: true,
            message: '数据状态不能为空',
            trigger: 'blur',
            type: 'number'
          }
        ]
      },

      lock: false,
      editLock: false,
      title: '新增',
      TYPENAME: '诊疗',
      detailTitle: this.TYPENAME,
      cascaderData: [],
      Specifications: [], // 单位
      FeeTypeId: [],
      itemId: -1, // 物品id
      // 库房类型id
      WAREHOUSEID: WAREHOUSEID
    }
  },
  mounted () {
    this.$nextTick(() => {
      this.getDicList().then(() => {
        this.$refs['item-file-template'].getItemList(1)
      })
    })
  },
  computed: {},
  mixins: [mixins],
  methods: {
    // 获取数据字典
    getDicList () {
      let apiList = [
        {
          url: 'SystemDictionary/DictionaryList',
          params: {
            typeId: FEETYPEID // 费用类别
          }
        },
        {
          url: 'Data/MedicalUnit/list',
          params: {
            unitType: 4, // 规格单位
            dataState: 1
          }
        }
      ]

      return this.swsApi
        .swsAllPost(apiList)
        .then(res => {
          this.FeeTypeId = res[0].data.result
          this.Specifications = res[1].data.result
        })
        .catch(e => {
          console.log(e)
        })
    },
    // 获取物品档案详情
    getItem () {
      this.detailLoading = true
      this.swsApi
        .swsPost(`Data/MedicalItemRecordView/${this.itemId}`)
        .then(res => {
          let data = res.data
          this.detailLoading = false
          if (!data.error) {
            data.result.medicalDrugExtension =
              data.result.medicalDrugExtension != null
                ? data.result.medicalDrugExtension
                : {
                  purchasingPrice: '',
                  retailPrice: ''
                }

            this.formValidate = data.result
          } else {
            this.$Message.error('网络错误，请稍后再试！')
          }
        })
    },
    save () {
      this.$refs['formValidate'].validate(valid => {
        if (valid) {
          this.addFlag = false
          let params = Object.assign({}, this.formValidate)
          this.title === '新增' && params.id && delete params.id
          params.forms && delete params.forms
          params.wareHouseId = params.wareHouseId.pop()
          params.medicalItemTypeEnum = 5
          this.swsApi
            .swsPost('Data/MedicalItemRecord/CreateUpdate', params)
            .then(res => {
              let data = res.data
              this.loading = false
              if (data.success) {
                this.$Message.success('操作成功')
                this.$refs['item-file-template'].getItemList(
                  this.$refs['item-file-template'].startPage
                )
                this.cancel('formValidate')
              } else {
                this.$Message.error(`操作失败,${data.error}`)
                this.cancel('formValidate')
              }
            })
            .catch(e => {
              console.log(e)
            })
        } else {
          this.$Message.error('请完善必填信息')
        }
      })
    },
    cascaderChange (v) {
      // this.wareHouseId = v.pop()
    }
  },
  components: {
    itemFileTemplate,
    Operate
  }
}
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
