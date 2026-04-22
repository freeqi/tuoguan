<template>
  <div id="comparison_review">
    <div class="top">
      <Form ref="compareH" :model="compareInfo.medicalItemInput" :rules="compareInfoV">
        <Row class="form" :gutter="10">
          <Col :lg="3" :md="3" :sm="3">
            <FormItem label="本地项目" prop="ypName">
              <Input type="text" v-model="compareInfo.medicalItemInput.ypName" placeholder="请输入"></Input>
            </FormItem>
          </Col>
          <Col :lg="3" :md="3" :sm="3">
            <FormItem label="商品名称" prop="brand">
              <Input type="text" v-model="compareInfo.medicalItemInput.brand" placeholder="请输入"></Input>
            </FormItem>
          </Col>
          <Col :lg="2" :md="2" :sm="2">
            <FormItem label="销售价" prop="salePrice">
              <InputNumber
                :min="0"
                style="width: 100%"
                v-model="compareInfo.medicalItemInput.salePrice"
                placeholder="请输入"
              ></InputNumber>
            </FormItem>
          </Col>
          <Col :lg="3" :md="3" :sm="3">
            <FormItem label="包装规格" prop="packaging">
              <Input type="text" v-model="compareInfo.medicalItemInput.packaging" placeholder="请输入"></Input>
            </FormItem>
          </Col>
          <Col :lg="4" :md="4" :sm="4">
            <FormItem label="生产厂家(非药品为单位)" prop="manufacturer">
              <Input
                type="text"
                v-model="compareInfo.medicalItemInput.manufacturer"
                placeholder="请输入"
              ></Input>
            </FormItem>
          </Col>
          <!-- <Col :lg="2" :md="2" :sm="2">
            <FormItem label="本地编码" prop="hiCenterCode">
              <Input type="text" readonly v-model="compareInfo.medicalItemInput.hiCenterCode"></Input>
            </FormItem>
          </Col> -->
          <Col :lg="4" :md="4" :sm="4">
            <FormItem label="国家项目编码" prop="nationItemCode">
              <Input type="text" v-model="compareInfo.medicalItemInput.nationItemCode"></Input>
            </FormItem>
          </Col>
          <Col :lg="2" :md="2" :sm="2">
            <FormItem label="目录等级" prop="hiLevel">
              <Input type="text" readonly v-model="compareInfo.medicalItemInput.hiLevel"></Input>
            </FormItem>
          </Col>
          <!-- <Button class="clearBtn" type="info" @click="clearMedicalInput('compareH')">清空</Button> -->
        </Row>
      </Form>
      <Form ref="compare" :model="compareInfo.si_YpmlQueryInput" :rules="compareInfoV">
        <Row class="form" :gutter="10">
          <Col :lg="3" :md="3" :sm="3">
            <FormItem label="医保中心项目" prop="ypName">
              <Input type="text" v-model="compareInfo.si_YpmlQueryInput.ypName" placeholder="请输入"></Input>
            </FormItem>
          </Col>
          <Col :lg="3" :md="3" :sm="3">
            <FormItem label="商品名称" prop="brand">
              <Input type="text" v-model="compareInfo.si_YpmlQueryInput.brand" placeholder="请输入"></Input>
            </FormItem>
          </Col>
          <Col :lg="2" :md="2" :sm="2">
            <FormItem label="销售价" prop="salePrice">
              <InputNumber
                :min="0"
                style="width: 100%"
                v-model="compareInfo.si_YpmlQueryInput.salePrice"
                placeholder="请输入"
              ></InputNumber>
            </FormItem>
          </Col>
          <Col :lg="3" :md="3" :sm="3">
            <FormItem label="包装规格" prop="packaging">
              <Input
                type="text"
                v-model="compareInfo.si_YpmlQueryInput.packaging"
                placeholder="请输入"
              ></Input>
            </FormItem>
          </Col>
          <Col :lg="4" :md="4" :sm="4">
            <FormItem label="生产厂家(非药品为单位)" prop="manufacturer">
              <Input
                type="text"
                v-model="compareInfo.si_YpmlQueryInput.manufacturer"
                placeholder="请输入"
              ></Input>
            </FormItem>
          </Col>
          <!-- <Col :lg="2" :md="2" :sm="2">
            <FormItem label="医保编码" prop="yplsh">
              <Input type="text" readonly v-model="compareInfo.si_YpmlQueryInput.yplsh"></Input>
            </FormItem>
          </Col> -->
          <Col :lg="4" :md="4" :sm="4">
            <FormItem label="国家项目编码" prop="gjypdm">
              <Input type="text" v-model="compareInfo.si_YpmlQueryInput.gjypdm"></Input>
            </FormItem>
          </Col>
          <Col :lg="2" :md="2" :sm="2">
            <FormItem label="目录等级" readonly prop="ylfydj">
              <Input type="text" readonly v-model="compareInfo.si_YpmlQueryInput.ylfydj"></Input>
            </FormItem>
          </Col>
          <!-- <Button class="clearBtn" type="info" @click="clearMedicalInput('compare')">清空</Button> -->
        </Row>
      </Form>
      <div class="button-group">
        <!-- <span>医保等级</span>
        <Select v-model="medical_insurance" placeholder="请选择医保等级" style="width: 160px;margin: 0 10px;">
          <Option
            v-for="(item, index) in medical_insurance_type"
            :value="index+1"
            :key="index"
          >{{item}}级</Option>
        </Select> -->

        <Input v-model="searchItemKey" placeholder="搜索名称/厂家..."  style="width: 170px;display: inline-table;margin-right:10px;"/>
        <Button type="primary" @click="searchReviewData">查询</Button>
        <Button @click="clearMedicalInput" >清空</Button>
      </div>
      <!-- <Divider/> -->
    </div>
    <div class="content">
      <Table
        ref="table_compared"
        :height="520"
        :loading="compared_loading"
        highlight-row
        :columns="compared_column"
        :data="computedItemData"
        no-data-text="暂无数据"
        @on-current-change="setOption"
      >
        <template slot-scope="{ row }" slot="action">
          <div>
            <Poptip placement="top-end" transfer width="260" v-if="row.auditState==0" content="审核">
              <Icon
                type="md-eye"
                size="22"
                color="#4f95e8"
                style="cursor: pointer;"
              ></Icon>
              <div slot="content">
                  审核状态： <Select v-model="auditObj.situation" transfer style="width: 70px;text-align:'left';margin:0 10px;" placeholder="请选择审核结果">
                    <Option :value="1">通过</Option>
                    <Option :value="2">拒绝</Option>
                  </Select>
                <Button type="primary" :loading="auditLoading" @click="saveAudit(row)">审核</Button>
              </div>
            </Poptip>
            <Tooltip v-else content="审核" placement="top" transfer>
              <Icon
                type="md-eye"
                size="22"
                color="grey"
                style="cursor: not-allowed"
              ></Icon>
            </Tooltip>
          </div>
        </template>
      </Table>
      <!-- <div class="pagination" v-show="dataCount > pageSize">
        <Page
          :total="dataCount"
          :page-size="pageSize"
          :current.sync="pageNum"
          @on-change="handleChangePage"
        />
      </div> -->
      
      <!-- 审核 -->
      <!-- <Modal v-model="auditModal" className="vertical-center-modal" width="400">
        <p slot="header" align="center">审核申请单</p>
        <Form :label-width="60" style="margin-bottom: 10px;">
          <FormItem label="审核状态">
            <Select v-model="auditObj.situation" style="width: 160px" placeholder="请选择审核结果">
              <Option :value="1">通过</Option>
              <Option :value="2">拒绝</Option>
            </Select>
          </FormItem>
        </Form>
        <div slot="footer" class="center">
          <Button type="primary" :loading="auditLoading" @click="saveAudit">审核</Button>
          <Button type="default" @click="auditModal = false">取消</Button>
        </div>
      </Modal> -->
    </div>
  </div>
</template>

<script>
import { toFilterKey } from '@/libs/tools'
const BUTTONROLE = {
  YPZD_DZ: 'YPZD_DZ',
  YPZD_QXDZ: 'YPZD_QXDZ'
}
export default {
  name: 'compare_review',
  data () {
    return {
      auditObj: {
        situation: 1
      },
      auditLoading: false,
      auditModal: false,
      searchItemKey: '',
      ydzList: [], // cy 20191209 保存 已对照的本地项目对应的医保中心项目list
      dataCount: 0, // 数据量
      pageNum: 1, // 页码
      pageSize: 9, // 每页数据条数
      medical_insurance_type: ['甲', '乙', '丙'],
      medical_insurance: 0,
      // 已对照
      compared_column: [
        {
          title: '项目名',
          key: 'medicalItemName',
          tooltip: true
        },
        {
          title: '商品名',
          key: 'spm',
          tooltip: true
        },
        {
          title: '剂型',
          key: 'dosageFormName',
          width: 100,
          tooltip: true
        },
        {
          title: '包装',
          key: 'packaging',
          tooltip: true
        },
        {
          title: '规格',
          key: 'specifications',
          tooltip: true
        },
        {
          title: '最小剂量',
          key: 'doseMin',
          tooltip: true
        },
        {
          title: '剂量单位',
          key: 'doseUnitName',
          tooltip: true
        },
        {
          title: '销售价',
          key: 'retailPrice',
          align: 'center',
          // render: (h, params) => {
          //   const {purchasingPrice, hilist_pric_uplmt_amt} = params.row
          //   if (purchasingPrice > hilist_pric_uplmt_amt && hilist_pric_uplmt_amt) {
          //     return (
          //       <span style="color: red">{purchasingPrice}</span>
          //     )
          //   } else {
          //     return (
          //       <span>{purchasingPrice || 0}</span>
          //     )
          //   }
          // }
        },
        {
          title: '医保中心价',
          render: (h, params) => {
            return (
              <span>{params.row.socialSecurityPrice || 0}</span>
            )
          }
        },
        {
          title: '等级',
          key: 'hiLevel',
          // render: (h, params) => {
          //   let list = ['/','甲','乙','丙']
          //   let index = parseInt(params.row.chrgitm_lv)
          //   return (
          //     <span>{isNaN(index)?list[0]:list[index]}</span>
          //   )
          // }
        },
        {
          title: '生产厂家',
          key: 'manufacturer',
          tooltip: true
        },
        // {
        //   title: '中心编码',
        //   key: 'hiCenterCode',
        //   tooltip: true
        // },
        {
          title: '国家项目编码',
          key: 'nationItemCode',
          width: 200,
          // tooltip: true
        },
        {
          title: '操作',
          slot: 'action',
          align: 'center'
        },
      ],
      compareInfo: {
        si_YpmlQueryInput: {
          ypName: '',
          brand: '',
          salePrice: null,
          packaging: '',
          manufacturer: '',
          yplsh: '',
          ylfydj: ''
        },
        medicalItemInput: {
          ypName: '',
          brand: '',
          salePrice: null,
          packaging: '',
          manufacturer: '',
          hiLevel: ''
        }
      },
      compareInfoV: {
        ypName: [],
        brand: [],
        salePrice: [],
        packaging: [],
        manufacturer: [],
        ylfydj: [],
        yplsh: []
      },
      compared_loading: false,
      compared_data: [],

      localItemId: '',
      centerItemId: '',
      // 项目对照ID
      typeID: 1,
      tabValue: '本地项目目录',
      buttonRole: BUTTONROLE
    }
  },
  // mixins: [columns],
  computed: {
		computedItemData(){
			let data = this.compared_data
			if(this.searchItemKey){
				data = toFilterKey(data,"medicalItemName,brand,manufacturer",this.searchItemKey);
			}
			return data
		},
  },
  mounted() {
    this.searchReviewData()
  },
  methods: {
    // handleChangePage () {
    //   this.searchComparedData()
    // },
    handleAudit(row){
      if(row.auditState!=0) return
      this.auditModal = true
    },
    saveAudit(row){
      let json = {...row}
      json.auditState = this.auditObj.situation
      console.log(json);
      this.auditLoading = true
      this.swsApi
        .swsPost(`Data/SI/NewHIMatchCode`,json)
        .then(res => {
          if (res.data.success) {
            this.$Notice.success({
              title:'审核成功',
              desc: res.data.KeyMsg||res.data.Msg,
            })
            this.searchReviewData()
          }
          this.auditLoading = false
        })
        .catch(e => {
            this.$Notice.warning({
              title:'操作失败',
              desc: res.data.KeyMsg||res.data.Msg,
            })
          this.auditLoading = false
        })
    },
    // showAudit(row){

    // },
    clearMedicalInput (name) {
      // this.$refs[name].resetFields()
      
      this.$refs.compare.resetFields()
      this.$refs.compareH.resetFields()
    },
    // 清空表单
    clearForm () {
      this.medical_insurance = 0
      this.$refs.compare.resetFields()
      this.$refs.compareH.resetFields()
      this.localItemId = ''
      this.centerItemId = ''
    },
    setOption (curRow) {
      console.log(curRow);
      if (curRow == null) return
      let { hiCenterCode,purchasingPrice, hiLevel, manufacturer,brand, goodsName,specifications, minDose, medicalItemName,nationItemCode} = curRow
      // this.compareInfo.medicalItemInput.hiCenterCode = hiCenterCode
      this.compareInfo.medicalItemInput.hiLevel = hiLevel
      this.compareInfo.medicalItemInput.manufacturer = manufacturer
      this.compareInfo.medicalItemInput.packaging = specifications
      this.compareInfo.medicalItemInput.brand = brand
      this.compareInfo.medicalItemInput.salePrice = purchasingPrice
      this.compareInfo.medicalItemInput.ypName = medicalItemName
      this.compareInfo.medicalItemInput.nationItemCode = nationItemCode//国家项目编码
      
      let { ycmc, hl, hldw, bzsl, bzdw, rl, rldw, ylfydj, yplsh, spm, ylbzdj, tym, gjypdm ,zxzjdw,medType} = curRow
      this.compareInfo.si_YpmlQueryInput.manufacturer = ycmc
      // this.compareInfo.si_YpmlQueryInput.packaging = `${rl ? rl + rldw + ':' : ''}${hl}${bzsl}${zxzjdw}/${bzdw}`

      this.compareInfo.si_YpmlQueryInput.packaging = `${medType==1?hl+bzsl+zxzjdw+'/'+bzdw:hl}`
      // this.compareInfo.si_YpmlQueryInput.yplsh = yplsh
      this.compareInfo.si_YpmlQueryInput.ylfydj = ylfydj
      this.compareInfo.si_YpmlQueryInput.brand = spm
      this.compareInfo.si_YpmlQueryInput.salePrice = ylbzdj
      this.compareInfo.si_YpmlQueryInput.ypName = tym
      this.compareInfo.si_YpmlQueryInput.gjypdm = gjypdm
    },
    // 获取已对照项目
    searchReviewData(){
      this.compared_loading = true
      this.swsApi
        .swsPost(`Data/SI/GetMedMatchCodeDataAsync`)
        .then(res => {
          if (res.data.success) {
            this.compared_data = res.data.result
            this.ydzList = res.data.result
          }
          this.compared_loading = false
        })
        .catch(e => {
          this.compared_loading = false
        })
    },
    // 获取已对照项目
    // searchComparedData () {
    //   this.compared_data = []
    //   this.dataCount = 0
    //   this.ydzList = []
    //   this.compared_loading = true
    //   let {ypName, brand, salePrice, packaging, manufacturer,nationItemCode} = this.compareInfo.medicalItemInput
    //   let params = {
    //     nationItemCode:nationItemCode,
    //     gjypdm:this.compareInfo.si_YpmlQueryInput.gjypdm,
    //     ypName: ypName,
    //     brand: brand,
    //     salePrice: salePrice,
    //     manufacturer: manufacturer,
    //     packaging: packaging,
    //     level: this.medical_insurance || 0,
    //     itemType: 1, // 1 表示药品对照
    //     pageSize: this.pageSize,
    //     pageNum: this.pageNum
    //   }
    //   this.swsApi
    //   // .swsGet(`Data/SI/YDZYPMLAsync/${this.typeID}`)
    //     .swsPost(`Data/SI/YDZYPMLAsync`, params)
    //     .then(res => {
    //       if (res.data.success) {
    //         this.compared_data = res.data.result.medical
    //         this.dataCount = res.data.dataCount
    //         this.ydzList = res.data.result.sIypmls
    //       }
    //       this.compared_loading = false
    //     })
    //     .catch(e => {
    //       this.compared_loading = false
    //     })
    // },
  },
  components: {}
}
</script>

<style scoped lang="less">
#comparison_review {
  padding: 0 0 10px;
  height: 100%;
  overflow-y: auto;
  background: #ffffff;
  .button-group {
    margin-top: 15px;
    text-align: left;
    button + button {
      margin-left: 10px;
    }
  }
  .top {
    z-index: 44;
    background: #ffffff;
    padding: 20px 40px 0;
    .ivu-form-item {
      // margin-bottom: 10px;
      margin-bottom: 0;
    }
    .ivu-form .ivu-form-item-label {
      line-height: 0.8;
    }
  }
  .bottom {
    z-index: 44;
    background: #ffffff;
    padding: 20px 40px 0;
    .ivu-form-item {
      margin-bottom: 10px;
    }
  }
  .content {
    margin-top: 20px;
    padding: 0 40px;
    .sec_header {
      margin-top: 20px;
      font-size: 16px;
      margin-bottom: 10px;
    }
  }
  .tab {
    margin: 0 0 30px;
  }
  .clearBtn {
    margin: 33px 0 0 5px;
  }
  & /deep/ .ivu-table .table-comparsion-row td {
    background-color: #c9f3e2;
  }
}
</style>
