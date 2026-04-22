<template>
  <div style="position: relative;">
    <Spin fix v-if="spinShow">
      <Icon type="ios-loading" size=18 class="demo-spin-icon-load"></Icon>
      <div>Loading</div>
    </Spin>
    <Card>
      <!-- 返回按钮 -->
      <Row>
        <Col :sm="1" :md="1" :lg="1">
          <Row>
            <i-col span="12" style="height:1px"></i-col>
            <i-col span="12">
              <Button icon="md-undo" @click="handleBack">返回</Button>
            </i-col>
          </Row>
        </Col>
        <i-col :sm="22" :md="22" :lg="22"></i-col>
        <i-col :sm="1" :md="1" :lg="1" style="height:1px"></i-col>
      </Row>
      <!-- title -->
      <Row>
        <i-col :sm="23" :md="23" :lg="23" style="text-align:center;margin-left:25px">
          <p class="title">{{operationName}}简介</p>
          <Divider class="divider"/>
        </i-col>
      </Row>
      <Row>
        <i-col :sm="23" :md="23" :lg="23" style="text-align:center;margin-left:25px">
          <i-col :sm="6" :md="6" :lg="6" style="text-align:left">
            <span class="yz">医院院长：{{operation_yz}}</span>
          </i-col>
          <i-col :sm="12" :md="12" :lg="12" style="height:1px"></i-col>
          <i-col :sm="6" :md="6" :lg="6" style="text-align:right">
            <span class="time">更新时间：{{time}}</span>
          </i-col>
        </i-col>
      </Row>
      <!-- 焦点图 -->
      <Row>
        <i-col :sm="1" :md="1" :lg="1" style="height:1px"></i-col>
        <i-col :sm="22" :md="22" :lg="22">
          <div id="operation_banner" v-if="listImg.length != 0">
            <Carousel
              :autoplay="setting.autoplay"
              :autoplay-speed="setting.autoplaySpeed"
              :dots="setting.dots"
              :radius-dot="setting.radiusDot"
              :trigger="setting.trigger"
              :arrow="setting.arrow">
              <CarouselItem v-for="(item, index) in listImg" :key="index">
                <div class="carousel_box">
                  <img id="carousel-img" :src="item">
                </div>
              </CarouselItem>
            </Carousel>
          </div>
        </i-col>
        <i-col :sm="1" :md="1" :lg="1" style="height:1px"></i-col>
      </Row>
      <!-- 简介 -->
      <div class="mgt-40">
        <div id="organization-describe-box" :sm="23" :md="23" :lg="23" v-html="describe"></div>
      </div>
      <Row>
        <i-col :sm="23" :md="23" :lg="23" class="mgt-40 mgl-25">
          <span style="font-size:18px">联系方式</span>
        </i-col>
      </Row>
      <Row>
        <i-col :sm="23" :md="23" :lg="23" class="mgl-25" style="margin-top:30px">
          <Row style="font-size:16px">
            <i-col :sm="24" :md="24" :lg="24">
              <i class="iconfont icon-dizhi" style="margin-right:12px"></i>医院地址 <span class="mgl-25">{{address}}</span>
            </i-col>
            <i-col :sm="24" :md="24" :lg="24" class="mgt-40">
              <Icon type="ios-call-outline" />
              {{phone}}
            </i-col>
          </Row>
        </i-col>
      </Row>
    </Card>
  </div>
</template>

<script>
import { mapMutations } from 'vuex'
export default {
  name: 'organizationDetails',
  props: {
    id: {
      type: Number
    }
  },
  data () {
    return {
      spinShow: true,
      operationName: '',
      operation_yz: '',
      address: '',
      phone: '',
      time: '',
      centerId: -1,
      describe: '',
      setting: {
        autoplaySpeed: 3000,
        dots: 'outside',
        radiusDot: false,
        trigger: 'click',
        arrow: 'hover'
      },
      // mapSetting: { // 地图
      //   center: [106.4902320000, 29.6330420000],
      //   zoom: 12,
      //   resizeEnable: true
      // },
      markers: [], // 地图标记
      listImg: [ ],
      loading: true
    }
  },
  created () {
    this.centerId = this.$route.params.id
  },
  mounted () {
    this.$nextTick(() => {
      this.handleGetData()
    })
  },
  watch: {
    // 如果路由有变化，会再次执行该方法
    '$route': 'handleGetData'
  },
  methods: {
    ...mapMutations([
      'closeTag'
    ]),
    // 点击返回按钮
    handleBack () {
      const {name, params} = this.$route
      this.closeTag({
        name,
        params
      })
      this.$router.push({
        name: 'organization_chart'
      })
    },
    // 获取透析中心详细数据
    handleGetData () {
      this.centerId = this.$route.params.id
      this.swsApi.swsPost(`CenterDialysis/DialysisDetails/${this.centerId}`)
        .then(res => {
          if (!res.data.error) {
            this.spinShow = false
          }
          const { dialysisAddress, dialysisContactMan, dialysisDetails, dialysisImgs, dialysisLat, dialysisLng, dialysisName, dialysisPhone, modifyTime } = res.data.result[0]

          this.operationName = dialysisName
          this.operation_yz = dialysisContactMan
          this.address = dialysisAddress
          this.phone = dialysisPhone
          this.time = modifyTime
          this.describe = dialysisDetails
          this.listImg = dialysisImgs
        })
    }
  }
}
</script>

<style scoped lang='less'>
.title{
  font-size: 18px;
  text-align:center;
  color: #333333;
  margin: 20px 0;
}
.divider{
  background: #999 !important;
}
.yz{
  font-size: 16px;
}
.time{
  font-size: 16px;
  color: #b6b6b6;
}
#operation_banner{
  height:464px;
  margin-top: 40px;
}
.carousel_box {
  height: 464px;
  #carousel-img {
    width: 100%;
    height: 100%;
    z-index: 999;
  }
}
.detail_map {
  width: 100%;
  height: 325px;
  margin: 20px 0;
}
.demo-spin-icon-load{
  animation: ani-demo-spin 1s linear infinite;
}
.mgt-40 {
  margin-top: 40px;
}
.mgl-25 {
  margin-left: 25px
}
</style>
