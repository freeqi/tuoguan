<template>
  <div id="medical_comparison">
    <div class="top">
      <!-- <div class="bottom"> -->
      <Form ref="compareH" :model="compareInfo.medicalItemInput" :rules="compareInfoV">
        <!-- <Row class="form" :gutter="10">
          <Col :lg="4" :md="4" :sm="4">
            <FormItem>
              <span>医保等级</span>
              <Select v-model="medical_insurance" placeholder="请选择医保等级">
                <Option
                  v-for="(item, index) in medical_insurance_type"
                  :value="index+1"
                  :key="index"
                >{{item}}级</Option>
              </Select>
            </FormItem>
          </Col>
        </Row> -->
        <Row class="form" :gutter="10">
          <Col :lg="4" :md="4" :sm="4">
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
            <FormItem label="规格" prop="packaging">
              <Input type="text" v-model="compareInfo.medicalItemInput.packaging" placeholder="请输入"></Input>
            </FormItem>
          </Col>
          <Col :lg="5" :md="6" :sm="6">
            <FormItem label="生产厂家(非药品显示为单位)" prop="manufacturer">
              <Input
                type="text"
                v-model="compareInfo.medicalItemInput.manufacturer"
                placeholder="请输入"
              ></Input>
            </FormItem>
          </Col>
          <Col :lg="3" :md="3" :sm="3">
            <FormItem label="本地编码" prop="hiCenterCode">
              <Input type="text" readonly v-model="compareInfo.medicalItemInput.hiCenterCode"></Input>
            </FormItem>
          </Col>
          <Col :lg="2" :md="2" :sm="2">
            <FormItem label="目录等级" prop="hiCenterLevel">
              <Input type="text" readonly v-model="compareInfo.medicalItemInput.hiCenterLevel"></Input>
            </FormItem>
          </Col>
        </Row>
      </Form>
      <Form ref="compare" :model="compareInfo.si_YpmlQueryInput" :rules="compareInfoV">
        <Row class="form" :gutter="10">
          <Col :lg="4" :md="4" :sm="4">
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
            <FormItem label="规格" prop="packaging">
              <Input
                type="text"
                v-model="compareInfo.si_YpmlQueryInput.packaging"
                placeholder="请输入"
              ></Input>
            </FormItem>
          </Col>
          <Col :lg="5" :md="6" :sm="6">
            <FormItem label="生产厂家(非药品显示为单位)" prop="manufacturer">
              <Input
                type="text"
                v-model="compareInfo.si_YpmlQueryInput.manufacturer"
                placeholder="请输入"
              ></Input>
            </FormItem>
          </Col>
          <Col :lg="3" :md="3" :sm="3">
            <FormItem label="医保编码" prop="yplsh">
              <Input type="text" readonly v-model="compareInfo.si_YpmlQueryInput.yplsh"></Input>
            </FormItem>
          </Col>
          <Col :lg="2" :md="2" :sm="2">
            <FormItem label="目录等级" readonly prop="ylfydj">
              <Input type="text" readonly v-model="compareInfo.si_YpmlQueryInput.ylfydj"></Input>
            </FormItem>
          </Col>
        </Row>
      </Form>
      <div class="button-group">
        <span>医保等级</span>
        <Select v-model="medical_insurance" placeholder="请选择医保等级" style="width: 160px;margin: 0 10px;">
          <Option
            v-for="(item, index) in medical_insurance_type"
            :value="index+1"
            :key="index"
          >{{item}}级</Option>
        </Select>
        <Button type="primary" @click="search">查询</Button>
        <Button type="primary" @click="compare" v-permission="buttonRole.YPZD_DZ">对照</Button>
        <Button type="default" @click="cancelCompare" v-show="tabValue === '已对照项目'" v-permission="buttonRole.YPZD_QXDZ">取消对照</Button>
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
        </TabPane>
      </Tabs>
    </div>
  </div>
</template>

<script>
import columns from './columns'
const BUTTONROLE = {
  YPZD_DZ: 'YPZD_DZ',
  YPZD_QXDZ: 'YPZD_QXDZ'
}
export default {
  data () {
    return {
      medical_insurance_type: ['甲', '乙', '丙'],
      medical_insurance: 0,
      compareInfo: {
        si_YpmlQueryInput: {
          ypName: '',
          brand: '',
          salePrice: 0,
          packaging: '',
          manufacturer: '',
          yplsh: '',
          ylfydj: ''
        },
        medicalItemInput: {
          ypName: '',
          brand: '',
          salePrice: 0,
          packaging: '',
          manufacturer: '',
          hiCenterCode: '',
          hiCenterLevel: ''
        }
      },
      compareInfoV: {
        ypName: [],
        brand: [],
        salePrice: [],
        packaging: [],
        manufacturer: [],
        hiCenterCode: [],
        hiCenterLevel: [],
        ylfydj: [],
        yplsh: []
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
      typeID: 1,
      tabValue: '本地项目目录',
      buttonRole: BUTTONROLE
    }
  },
  mixins: [columns],
  methods: {
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
            this.clearForm()
            this.search()
          } else {
            this.$Message.error('操作失败，请稍后再试！')
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
      if (curRow.hasOwnProperty('bgsj')) {
        let { ycmc, hl, ylfydj, yplsh, spm, ylbzdj, id, tym } = curRow

        this.compareInfo.si_YpmlQueryInput.manufacturer = ycmc
        this.compareInfo.si_YpmlQueryInput.packaging = hl
        this.compareInfo.si_YpmlQueryInput.yplsh = yplsh
        this.compareInfo.si_YpmlQueryInput.ylfydj = ylfydj
        this.compareInfo.si_YpmlQueryInput.brand = spm
        this.compareInfo.si_YpmlQueryInput.salePrice = ylbzdj
        this.compareInfo.si_YpmlQueryInput.ypName = tym
        this.centerItemId = id
      } else {
        let {
          manufacturer,
          goodsName,
          minDose,
          id,
          medicalItemName
        } = curRow

        this.compareInfo.medicalItemInput.manufacturer = manufacturer
        this.compareInfo.medicalItemInput.packaging = minDose
        this.compareInfo.medicalItemInput.brand = goodsName
        this.compareInfo.medicalItemInput.salePrice =
          curRow.medicalDrugExtension.purchasingPrice
        this.compareInfo.medicalItemInput.ypName = medicalItemName
        this.localItemId = id
      }
    },
    // 查询
    search () {
      if (this.tabValue === '本地项目目录') {
        this.searchLocal()
      } else {
        this.searchComparedData()
      }
    },
    // 查询本地项目
    searchLocal () {
      let params = JSON.parse(JSON.stringify(this.compareInfo))

      params.si_YpmlQueryInput.level = this.medical_insurance
      params.medicalItemInput.level = this.medical_insurance

      this.loading = true
      this.center_loading = true
      this.swsApi
        .swsPost('Data/SI/YPMLAsync', params)
        .then(res => {
          if (res.data.success) {
            this.local_data = res.data.result.medical
            this.center_data = res.data.result.sIypmls
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
      this.swsApi
        .swsGet(`Data/SI/YDZYPMLAsync/${this.typeID}`)
        .then(res => {
          if (res.data.success) {
            this.compared_data = res.data.result.medical
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
        setTimeout(() => {
          this.searchComparedData()
        }, 600)
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
}
</style>
