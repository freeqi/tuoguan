<template>
  <detail-info ref="detail">
    <!-- 数据请求完成前显示加载 -->
    <spin size="large" fix v-if="loading"></spin>
    <div v-else>
      <div class="info-box">
        <div class="main-info">
          <div>
            <Icon type="ios-person-outline" size="24" />
            <span>{{staff.name}}</span>
          </div>
          <div>
            <Icon v-if="sex === '男'" type="ios-male" size="18" color="#4f95e8" />
            <Icon v-else type="ios-female" size="18" color="#ff7f91" />
            <span>{{staff.age}}岁</span>
          </div>
          <div>
            <Icon type="ios-call-outline" size="18" />
            <span>{{staff.phone}}</span>
          </div>
        </div>
        <div class="detail">
          <p>
            <span class="field">
              <label>民族：</label>
              {{staff.ethnic}}
            </span>
            <span class="field">
              <label>出生日期：</label>
              {{staff.birthday}}
            </span>
          </p>
          <p>
            <span class="field">
              <label>身份证号码：</label>
              {{staff.idcard}}
            </span>
          </p>
          <p>
            <span class="field">
              <label>紧急联系方式：</label>
              {{staff.emergencyPhone}}
            </span>
          </p>
          <p>
            <span class="field">
              <label>户口所在地：</label>
              {{staff.hjAddress}}
            </span>
          </p>
          <p>
            <span class="field">
              <label>现居住地：</label>
              {{staff.contactAddress}}
            </span>
          </p>
        </div>
      </div>
      <!-- 在职情况 -->
      <div class="info-box">
        <div class="main-info">
          <div>
            <Icon type="ios-card-outline" size="24" />
            <span>在职基本情况</span>
          </div>
        </div>
        <div class="detail">
          <p>
            <span class="field">
              <label>机构：</label>
              {{staff.centerDialysisName}}
            </span>
          </p>
          <p>
            <span class="field">
              <label>部门：</label>
              {{staff.department}}
            </span>
            <span class="field">
              <label>职务：</label>
              <span class="blue">{{staff.position}}</span>
            </span>
            <!-- v-if="['医生','主任','护士','护士长'].includes(staff.position)" -->
            <span class="field" >
              <label>医师编码：</label>
              <span>{{staff.nationDoctCode}}</span>
            </span>
          </p>
          <p>
            <span class="field">
              <label>入职日期：</label>
              {{staff.hiredate}}
            </span>
            <span class="field">
              <label>转正日期：</label>
              {{staff.positiveDate}}
            </span>
          </p>
          <p>
            <span class="field">
              <label>签订合同日期：</label>
              {{staff.signDate}}
            </span>
          </p>
          <p>
            <span class="field">
              <label>合同到期日期：</label>
              {{staff.expireDAte}}
            </span>
          </p>
          <p>
            <span class="field">
              <label>外派合同情况：</label>
              {{staff.assignmentStatus}}
            </span>
            <span class="field">
              <label>户口性质：</label>
              {{staff.householdRegister}}
            </span>
          </p>
          <p>
            <span class="field">
              <label>劳动合同关系：</label>
              {{staff.laborContract}}
            </span>
          </p>
          <p>
            <span class="field">
              <label>举荐人：</label>
              {{staff.recommendations}}
            </span>
            <span class="field">
              <label>联系电话：</label>
              {{staff.recommendationsPhone}}
            </span>
          </p>
        </div>
      </div>
      <!-- 学历专业信息 -->
      <div class="info-box">
        <div class="main-info">
          <div>
            <Icon type="ios-card-outline" size="24" />
            <span>学历专业信息</span>
          </div>
        </div>
        <div class="detail">
          <p>
            <span class="field">
              <label>毕业院校：</label>
              {{staff.highestSchool}}
            </span>
            <span class="field">
              <label>毕业时间：</label>
              {{staff.graduationDate}}
            </span>
          </p>
          <p>
            <span class="field">
              <label>最高学历：</label>
              {{staff.educationBackground}}
            </span>
          </p>
          <p>
            <span class="field">
              <label>专业：</label>
              {{staff.major}}
            </span>
            <span class="field">
              <label>职称：</label>
              {{staff.jobTitle}}
            </span>
          </p>
        </div>
      </div>
      <!-- 工作年龄 -->
      <div class="info-box">
        <div class="main-info">
          <div>
            <Icon type="ios-list-box-outline" size="24" />
            <span>工作年龄</span>
            <span class="blue">（{{staff.jobYear}}年）</span>
          </div>
        </div>
        <div class="detail">
          <Timeline style="margin-top: 20px;">
            <TimelineItem v-for="(item, index) in staff.experienceJobs" :key="item+index">
              <p class="content">{{item}}</p>
            </TimelineItem>
          </Timeline>
        </div>
      </div>
      <!-- 工作调动记录 -->
      <div class="info-box">
        <div class="main-info">
          <div>
            <Icon type="ios-list-box-outline" size="24" />
            <span>调动记录</span>
          </div>
        </div>
        <div class="detail">
          <Timeline style="margin-top: 20px;">
            <TimelineItem v-for="(item, index) in transfer" :key="item+index">
              <p class="content">{{item.strEmployeeTransfer}}</p>
            </TimelineItem>
          </Timeline>
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
      sex: '',
      transfer: []
    }
  },
  props: {
    staff: {
      require: true
    },
    sSex: {
      require: true
    }
  },
  watch: {
    staff () {
      this.sex = this.sSex.filter(v => {
        return v.id === this.staff.sex
      })[0].name
      this.mobility_record(this.staff.id)
    }
  },
  methods: {
    show () {
      this.$refs.detail.show()
    },
    hide () {
      this.$refs.detail.hide()
    },
    // 调动记录
    mobility_record (id) {
      this.swsApi.swsPost(`Employee/Employee/Transfers/${id}`).then(({data: {result, success}}) => {
        if (success) {
          this.transfer = result
        }
      })
    }
  },
  components: {
    detailInfo
  }
}
</script>

<style scoped lang="less">
.timeLine {
  padding-left: 140px;
  i {
    display: inline-block;
    width: 8px;
    height: 8px;
    border-radius: 8px;
    background: #c5c5c5;
  }
  /deep/ .ivu-timeline-item-head-custom {
    padding: 0;
    line-height: 8px;
  }
  & /deep/ .ivu-timeline-item-content {
    padding-bottom: 20px;
  }
  .color-info {
    color: #5c9dea;
  }
  .color-error {
    color: #fc4b4b;
  }
  .left {
    text-align: right;
    position: absolute;
    left: -10px;
    transform: translateX(-100%);
    .time {
      margin-top: 2px;
    }
    & p:first-child {
      margin-top: 0;
    }
  }
  .content {
    & > * {
      margin-bottom: 2px;
    }
    .list,
    .solve {
      color: #999;
    }
    .list + .people {
      margin-top: 10px;
    }
    .money {
      color: #fc4b4b;
    }
  }
}
</style>
