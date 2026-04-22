<template>
  <detail-info ref="detail" :title="title">
    <!-- 数据请求完成前显示加载 -->
    <spin size="large" fix v-if="loading"></spin>
    <div v-else>
      <div class="info-box">
        <div class="main-info">
          <div>
            <Icon type="ios-person-outline" size="24"/>
            <span>{{company.name}}</span>
          </div>
        </div>
        <div class="detail">
          <p>
            <span class="field"><label>供应商编码：</label>{{company.supCode}}</span>
          </p>
          <p>
            <span class="field"><label>供应商地址：</label>{{company.address}}</span>
          </p>
          <p>
            <span class="field"><label>联系人：</label>{{company.linkMan}}</span>
          </p>
          <p>
            <span class="field"><label>联系电话：</label>{{company.phone}}</span>
          </p>
          <p>
            <span class="field"><label>法人代表：</label>{{company.legalPerson}}</span>
          </p>
          <p>
            <span class="field"><label>主营业务：</label>{{company.mainBusiness}}</span>
          </p>
          <p>
            <span class="field flex">
              <label>备注：</label>
              <div>{{company.remark}}</div>
            </span>
          </p>
          <p>
            <span class="field flex">
              <label>主营类别：</label>
              <div v-if="company.mainCategories">
                <p v-for="(item, index) in mainCategories" :key="index"><Icon type="ios-checkmark" size="24" color="#5ea8ff" />{{item}}</p>
              </div>
            </span>
          </p>
          <p>
            <span class="field flex">
              <label>证件照：</label>
              <div style="padding-top:10px">
                <img :src="company.image1" v-if="company.image1" alt="">
                <img :src="company.image2" v-if="company.image2" alt="">
              </div>
            </span>
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
      title: '详细信息'
    }
  },
  props: {
    company: {
      type: Object,
      default: () => {
        return {}
      }
    }
  },
  computed: {
    mainCategories () {
      if (!this.company.mainCategories) return []
      return this.company.mainCategories.split(',')
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
.flex {
  display: flex;
  width: 100%;
  div {
    flex: 1;
    img:first-child {
      margin-right: 10px
    }
    img {
      width: 138px;
      height: 93px;
    }
  }
}

</style>
