<template>
  <div id="medical_comparison">
    <div class="top">
      <!-- <div class="bottom"> -->
      <Form ref="compareH" :model="compareInfo.medicalItemInput">
        <!-- <Row class="form" :gutter='10'>
          <Col :lg="4" :md="4" :sm="4">
            <FormItem >
              <span>医保等级</span>
              <Select v-model="medical_insurance" placeholder="请选择医保等级">
                <Option v-for="(item, index) in medical_insurance_type" :value="index+1" :key="index">{{item}}级</Option>
              </Select>
            </FormItem>
          </Col>
        </Row>-->
        <Row class="form" :gutter="10">
          <Col :lg="4" :md="4" :sm="4">
            <FormItem label="本地项目" prop="ypName">
              <Input type="text" v-model="compareInfo.medicalItemInput.ypName" placeholder="请输入"></Input>
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
            <FormItem label="规格" prop="packaging">
              <Input type="text" v-model="compareInfo.medicalItemInput.packaging" placeholder="请输入"></Input>
            </FormItem>
          </Col>
          <!-- <Col :lg="3" :md="3" :sm="3">
            <FormItem label="本地编码" prop="hiCenterCode">
              <Input type="text" readonly v-model="compareInfo.medicalItemInput.hiCenterCode"></Input>
            </FormItem>
          </Col> -->
          <Col :lg="6" :md="6" :sm="6">
            <FormItem label="国家项目编码" prop="nationItemCode">
              <Input type="text" v-model="compareInfo.medicalItemInput.nationItemCode"></Input>
            </FormItem>
          </Col>
          <Col :lg="3" :md="3" :sm="3">
            <FormItem label="生产厂家" prop="manufacturer">
              <Input
                type="text"
                v-model="compareInfo.medicalItemInput.manufacturer"
                placeholder="请输入"
              ></Input>
            </FormItem>
          </Col>
          <Col :lg="2" :md="2" :sm="2">
            <FormItem label="目录等级" prop="hiCenterLevel">
              <Input type="text" readonly v-model="compareInfo.medicalItemInput.hiCenterLevel"></Input>
            </FormItem>
          </Col>
          <Button class="clearBtn" type="info" @click="clearMedicalInput('compareH')">清空</Button>
        </Row>
      </Form>
      <Form ref="compare" :model="compareInfo.si_YpmlQueryInput">
        <Row class="form" :gutter="10">
          <Col :lg="4" :md="4" :sm="4">
            <FormItem label="医保中心项目" prop="ypName">
              <Input type="text" v-model="compareInfo.si_YpmlQueryInput.ypName" placeholder="请输入"></Input>
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
            <FormItem label="规格" prop="packaging">
              <Input
                type="text"
                v-model="compareInfo.si_YpmlQueryInput.packaging"
                placeholder="请输入"
              ></Input>
            </FormItem>
          </Col>
          <!-- <Col :lg="3" :md="3" :sm="3">
            <FormItem label="医保编码" prop="yplsh">
              <Input type="text" readonly v-model="compareInfo.si_YpmlQueryInput.yplsh"></Input>
            </FormItem>
          </Col> -->
          <Col :lg="6" :md="6" :sm="6">
            <FormItem label="国家项目编码" prop="gjxmdm">
              <Input type="text" v-model="compareInfo.si_YpmlQueryInput.gjxmdm"></Input>
            </FormItem>
          </Col>
          <Col :lg="3" :md="3" :sm="3">
            <FormItem label="生产厂家" prop="bz">
              <Input
                type="text"
                v-model="compareInfo.si_YpmlQueryInput.bz"
                placeholder="请输入"
              ></Input>
            </FormItem>
          </Col>
          <Col :lg="2" :md="2" :sm="2">
            <FormItem label="目录等级" readonly prop="ylfydj">
              <Input type="text" readonly v-model="compareInfo.si_YpmlQueryInput.ylfydj"></Input>
            </FormItem>
          </Col>
          <Button class="clearBtn" type="info" @click="clearMedicalInput('compare')">清空</Button>
        </Row>
      </Form>
      <div class="button-group">
        <span>医保等级</span>
        <Select
          v-model="medical_insurance"
          placeholder="请选择医保等级"
          style="width: 160px;margin: 0 10px;"
        >
          <Option
            v-for="(item, index) in medical_insurance_type"
            :value="index+1"
            :key="index"
          >{{item}}级</Option>
        </Select>
        <Button type="primary" @click="search">查询</Button>
        <Button type="success" @click="compare" v-permission="buttonRole.ZLXMZD_DZ">对照</Button>
        <Button
          type="default"
          @click="cancelCompare"
          v-show="tabValue == '已对照项目'" v-permission="buttonRole.ZLXMZD_QXDZ"
        >取消对照</Button>
        <Button type="primary" @click="update">更新诊疗项目目录</Button>
      </div>
      <Divider/>
    </div>
    <div class="content">
      <Tabs type="card" class="tab" @on-click="changeTab" :value="tabValue">
        <TabPane label="本地项目目录" name="本地项目目录">
          <Table
            ref="table_local"
            :height="240"
            :loading="loading"
            highlight-row
            :columns="local_column"
            :data="local_data"
            no-data-text="暂无数据"
            @on-current-change="setOption"
          ></Table>
          <h3 class="sec_header">医保中心目录</h3>
          <Table
            ref="table_center"
            :height="240"
            highlight-row
            :row-class-name="setComparsionRowColor"
            :loading="center_loading"
            :columns="center_column"
            :data="center_data"
            no-data-text="暂无数据"
            @on-current-change="setOption"
          ></Table>
        </TabPane>
        <TabPane label="已对照项目" name="已对照项目">
          <Table
            ref="table_local"
            :height="480"
            :loading="compared_loading"
            highlight-row
            :columns="compared_column"
            :data="compared_data"
            no-data-text="暂无数据"
            @on-current-change="setOption"
          ></Table>
          <div class="pagination" v-show="dataCount > pageSize">
            <Page
              :total="dataCount"
              :page-size="pageSize"
              :current.sync="pageNum"
              @on-change="handleChangePage"
            />
          </div>
        </TabPane>
      </Tabs>
    </div>
  </div>
</template>

<script>
import columns from './columns'
const BUTTONROLE = {
  ZLXMZD_DZ: 'ZLXMZD_DZ',
  ZLXMZD_QXDZ: 'ZLXMZD_QXDZ'
}
export default {
  data () {
    return {
      ydzList: [], // cy 20191209 保存 已对照的本地项目对应的医保中心项目list
      dataCount: 0, // 数据量
      pageNum: 1, // 页码
      pageSize: 9, // 每页数据条数
      medical_insurance_type: ['甲', '乙', '丙'],
      medical_insurance: 0,
      compareInfo: {
        si_YpmlQueryInput: {
          ypName: '',
          salePrice: null,
          packaging: '',
          yplsh: '',
          ylfydj: ''
        },
        medicalItemInput: {
          ypName: '',
          salePrice: null,
          packaging: '',
          hiCenterCode: '',
          hiCenterLevel: ''
        }
      },
      loading: false,
      center_loading: false,
      compared_loading: false,

      local_data: [],
      center_data: [],
      compared_data: [],

      localItemId: '',
      centerItemId: '',
      // 项目对照ID
      typeID: 5,
      tabValue: '本地项目目录',
      buttonRole: BUTTONROLE
    }
  },
  mixins: [columns],
  methods: {
    update(){
      this.swsApi.swsPost("Data/SI/FWML").then(res=>{
        if (res.data.success) {
            this.$Message.success({
              content: res.data.result.msg,
              duration: 2
            })
          }
      })
    },
    handleChangePage () {
      // this.clearMedicalInput('compare')
      // this.clearMedicalInput('compareH')
      this.searchComparedData()
    },
    // 在医保中心 项目table中标识对应的已对照本地项目
    setComparsionRowColor (row) {
      if (this.local_data.length !== 0) {
        let arr = this.local_data.filter(item => {
          return item.nationItemCode !== null && item.nationItemCode === row.gjxmdm
        })
        if (arr.length > 0) return 'table-comparsion-row'
      }
    },
    // 清空查询条件
    clearMedicalInput (name) {
      this.$refs[name].resetFields()
    },
    // 取消对照
    cancelCompare () {
      if (!this.localItemId) {
        this.$Message.error('请选择项目')
        return
      }
      this.swsApi
        .swsPost(`Data/SI/Clear/${this.localItemId}`)
        .then(res => {
          if (res.data.success) {
            this.$Message.success({
              content: '取消对照成功！',
              duration: 2
            })

            if (this.tabValue === '已对照项目') {
              this.searchComparedData()
            } else {
              this.search()
            }
          }
        })
        .catch(e => {
          console.log(e)
        })
      this.clearForm()
    },
    // 清空表单
    clearForm () {
      this.medical_insurance = 0
      this.$refs.compare.resetFields()
      this.$refs.compareH.resetFields()

      this.$refs.table_local.clearCurrentRow()
      this.$refs.table_center.clearCurrentRow()

      this.localItemId = ''
      this.centerItemId = ''
    },
    // 对照
    compare () {
      if (!this.localItemId) {
        this.$Message.error('请选择本地项目')
        return
      }
      if (!this.centerItemId) {
        this.$Message.error('请选择医保中心项目')
        return
      }
      this.swsApi
        .swsPost(
          `Data/SI/YPMLAsync/${this.localItemId}/${this.centerItemId}/${
            this.typeID
          }`
        )
        .then(res => {
          if (res.data.success) {
            this.$Message.success({
              content: '对照成功',
              duration: 3
            })
            // this.clearForm()
            this.search()
          } else {
            this.$Message.error(`操作失败，${res.data.error}`)
          }
        })
        .catch(e => {
          console.log(e)
          this.loading = false
          this.center_loading = false
        })
    },
    setOption (curRow) {
      if (curRow == null) return
      console.log(curRow);
      if (curRow.hasOwnProperty('bgsj')) {
        let { dw, ylfydj, xmlsh, ylbzj, id, xmmc, gjxmdm, bz } = curRow

        this.compareInfo.si_YpmlQueryInput.packaging = dw
        this.compareInfo.si_YpmlQueryInput.yplsh = xmlsh
        this.compareInfo.si_YpmlQueryInput.ylfydj = ylfydj
        this.compareInfo.si_YpmlQueryInput.salePrice = ylbzj
        this.compareInfo.si_YpmlQueryInput.ypName = xmmc
        this.compareInfo.si_YpmlQueryInput.gjxmdm = gjxmdm
        this.compareInfo.si_YpmlQueryInput.bz = bz
        this.centerItemId = id
      } else {
        let { hiCenterCode, hiLevel, minDose, id, medicalItemName, nationItemCode,manufacturer,purchasingPrice } = curRow

        // this.compareInfo.medicalItemInput.hiCenterCode = hiCenterCode
        this.compareInfo.medicalItemInput.hiCenterLevel = hiLevel
        this.compareInfo.medicalItemInput.packaging = minDose
        this.compareInfo.medicalItemInput.salePrice = purchasingPrice
        this.compareInfo.medicalItemInput.ypName = medicalItemName
        this.compareInfo.medicalItemInput.nationItemCode = nationItemCode
        this.compareInfo.medicalItemInput.manufacturer = manufacturer
        
        this.localItemId = id
        if (this.tabValue === '已对照项目') {
          let data = this.ydzList.filter(res => {
            // 将已对照的项目的中心编码和医保中心的编码匹配
            return res.gjxmdm === nationItemCode
          })
          if (data.length > 0) {
            let { dw, ylfydj, xmlsh, ylbzj, id, xmmc, bz,gjxmdm } = data[0]
            this.compareInfo.si_YpmlQueryInput.packaging = dw
            this.compareInfo.si_YpmlQueryInput.yplsh = xmlsh
            this.compareInfo.si_YpmlQueryInput.ylfydj = ylfydj
            this.compareInfo.si_YpmlQueryInput.salePrice = ylbzj
            this.compareInfo.si_YpmlQueryInput.ypName = xmmc
            this.compareInfo.si_YpmlQueryInput.gjxmdm = gjxmdm
            this.compareInfo.si_YpmlQueryInput.bz = bz
            this.centerItemId = id
          }
        }
      }
    },
    // 查询
    search () {
      this.pageNum = 1
      if (this.tabValue === '本地项目目录') {
        this.searchLocal()
      } else {
        this.searchComparedData()
      }
    },
    // 查询本地项目
    searchLocal () {
      let params = {
        gjxmdm:this.compareInfo.si_YpmlQueryInput.gjxmdm,
        nationItemCode:this.compareInfo.medicalItemInput.nationItemCode,
        bdItemName: this.compareInfo.medicalItemInput.ypName,
        zxItemName: this.compareInfo.si_YpmlQueryInput.ypName,
        Manufacturer:this.compareInfo.medicalItemInput.manufacturer,
        bz:this.compareInfo.si_YpmlQueryInput.bz
      }

      this.loading = true
      this.center_loading = true
      this.swsApi
        .swsPost('Data/SI/NewFWXM', params)
        .then(res => {
          if (res.data.success) {
            this.local_data = res.data.result.medical
            this.center_data = res.data.result.sI_HCOutPuts
          } else {
            this.$Message.error('查询失败')
          }
          this.loading = false
          this.center_loading = false
        })
        .catch(e => {
          console.log(e)
          this.loading = false
          this.center_loading = false
        })
    },
    // 获取已对照项目
    searchComparedData () {
      this.compared_loading = true
      this.compared_data = []
      this.dataCount = 0
      this.ydzList = []
      let {ypName, brand, salePrice, packaging, manufacturer,nationItemCode} = this.compareInfo.medicalItemInput
      let params = {
        gjxmdm:this.compareInfo.si_YpmlQueryInput.gjxmdm,
        nationItemCode:nationItemCode,
        ypName: ypName,
        brand: brand,
        salePrice: salePrice,
        manufacturer: manufacturer,
        packaging: packaging,
        level: this.medical_insurance || 0,
        // itemType: 2, // 2 表示其他对照
        itemType: 5, // 20211216 cy 诊疗项目的已对照 改为 5
        pageSize: this.pageSize,
        pageNum: this.pageNum
      }
      this.swsApi
      // .swsGet(`Data/SI/YDZYPMLAsync/${this.typeID}`)
        .swsPost(`Data/SI/YDZYPMLAsync`, params)
        .then(res => {
          if (res.data.success) {
            this.compared_data = res.data.result.medical
            this.dataCount = res.data.dataCount
            this.ydzList = res.data.result.sI_HCOutPuts
          }
          this.compared_loading = false
        })
        .catch(e => {
          this.compared_loading = false
        })
    },
    changeTab (name) {
      this.tabValue = name
      if (name === '已对照项目') {
        this.searchComparedData()
      } else {
        this.clearForm()
      }
    }
  },
  components: {}
}
</script>

<style scoped lang="less">
#medical_comparison {
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
