<template>
  <detail-info ref="detail" :title="title">
    <!-- 数据请求完成前显示加载 -->
    <spin size="large" fix v-if="loading"></spin>
    <div v-else>
      <div class="info-box">
        <div class="main-info">
          <div>
            <Icon type="ios-person-outline" size="24"/>
            <span>{{patient.name}}</span>
          </div>
          <div>
            <Icon v-if="patient.sex == '男'" type="ios-male" size="18" color="#4f95e8"/>
            <Icon v-else type="ios-female" size="18" color="#ff7f91" />
            <span>{{patient.age}}岁</span>
          </div>
        </div>
        <div class="detail">
          <p>
            <span class="field"><label>民族：</label>{{patient.nation}}</span>
            <span class="field"><label>出生日期：</label>{{patient.birthday}}</span>
          </p>
          <p>
            <span class="field"><label>所在医院：</label>{{patient.dialysis}}</span>
          </p>
          <p>
            <span class="field"><label>证件号码：</label>{{patient.cardNum}}</span>
          </p>
          <p>
            <span class="field"><label>职业：</label>{{patient.professional}}</span>
          </p>
          <p>
            <span class="field"><label>工作单位：</label>{{patient.workUnit}}</span>
          </p>
          <p>
            <span class="field"><label>联系地址：</label>{{patient.censusRegister}}</span>
          </p>
          <p>
            <span class="field"><label>户籍所在地：</label>{{patient.contactAddress}}</span>
          </p>
        </div>
      </div>
      <!-- 在职情况 -->
      <div class="info-box">
        <div class="main-info">
          <div>
            <Icon type="ios-contacts-outline" size="24" />
            <span>联系方式</span>
          </div>
        </div>
        <div class="detail">
          <p>
            <span class="field"><label>本人电话：</label>{{patient.contactPhone}}</span>
          </p>
          <p>
            <span class="field"><label>紧急联系人：</label>{{patient.emergencyContact1}}</span>
            <span class="field"><label>所属关系：</label>{{patient.relationship1}}</span>
          </p>
          <p>
            <span class="field"><label>紧急联系电话：</label>{{patient.emergencyContactNumber1}}</span>
          </p>
          <p>
            <span class="field"><label>联系地址：</label>{{patient.contactAddress}}</span>
          </p>
        </div>
      </div>
      <!-- 诊疗信息 -->
      <div class="info-box">
        <div class="main-info">
          <div>
            <Icon type="ios-card-outline" size="24" />
            <span>诊疗信息</span>
          </div>
        </div>
        <div class="detail">
          <p>
            <span class="field"><label>患者编号：</label><span class="blue">{{patient.patientNo}}</span></span>
          </p>
          <p>
            <span class="field"><label>血源性疾病：</label>{{patient.sBloodBorneDisease}}</span>
          </p>
          <p>
            <span class="field"><label>接诊医生：</label>{{patient.receiveDoctor}}</span>
          </p>
          <p>
            <span class="field"><label>医保类型：</label>{{patient.ssiInsuredType}}</span>
          </p>
        </div>
      </div>
    </div>
  </detail-info>
</template>

<script>
import detailInfo from '@/components/detail-info/'
export default {
  data () {
    return {
      flage: false,
      loading: false,
      title: '患者详情'
    }
  },
  props: {
    patient: {
      type: Object,
      default: () => {
        return {}
      }
    }
  },
  methods: {
    show () {
      this.$refs.detail.show()
    },
    hide () {
      this.$refs.detail.hide()
    }
  },
  components: {
    detailInfo
  }
}
</script>

<style scoped lang="less">

</style>
