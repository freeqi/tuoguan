<template>
  <div id="staff">
    <parent-view2 @on-change="changeHospital" ref="tree">
      <div slot="content">
        <div class="chart" ref="chart">
          <div class="btn-group">
            <Select
              v-model="statisticTypeModel"
              @on-change="changeChartType"
              style="width:200px; margin-left: 10px;"
            >
              <Option
                :value="item.type"
                v-for="item in statisticType"
                :key="item.type"
              >按{{item.name}}分类</Option>
            </Select>
            <Button
              shape="circle"
              icon="md-cloud-upload"
              style="margin-left: 10px;"
              @click="download"
              v-permission="buttonRole.ZGGL_DC"
            >导出</Button>
          </div>
          <div ref="dom" style="height: 100%;"></div>
        </div>
        <div class="staff-operate">
          <div class="operate">
            <div>
              <Button
                class="add-btn"
                type="primary"
                @click="add()"
                icon="md-add"
                v-permission="buttonRole.ZGGL_XZRY"
              >新增人员</Button>
              <sws-upload
                :importId="importId"
                v-permission="buttonRole.ZGGL_DR"
                @on-success-upload="handleUploadSuccess"
              ></sws-upload>
              <RadioGroup v-model="empSelectType" @on-change="empTypeChange" type="button" style="padding: 0 10px;">
                <Radio :label="0">全部</Radio>
                <Radio :label="1">医生</Radio>
                <Radio :label="2">护士</Radio>
                <Radio :label="3">医护</Radio>
              </RadioGroup>
            </div>
            <Input
              v-model="searchKey"
              search
              enter-button
              @on-search="searchStaff"
              placeholder="请输入关键字"
              style="width:230px;"
            />
          </div>
          <swsTable
            class="sws-table"
            style="margin-top: 20px;"
            ref="table"
            :loading="loading"
            :columns="table_column"
            :data="staff_data"
            :pagination="pagination"
            @on-change="changePage"
            :row-class-name="rowClassName"
          />
        </div>

        <transition name="fade">
          <staff-detail id="staff-detail" :sSex="sSex" :staff="staffDetail" ref="staffDetail"></staff-detail>
        </transition>

        <!-- 删除人员 -->
        <Modal v-model="delModal" width="400" class-name="vertical-center-modal">
          <p slot="header">
            <span>删除人员</span>
          </p>
          <div style="text-align:center; padding: 40px 0; font-size: 16px;">
            <p>
              <Icon type="ios-help-circle" color="#ff81a3" size="30" style="margin-right: 20px;" />删除后不可恢复，您确定删除吗？
            </p>
          </div>
          <div slot="footer" style="text-align: center">
            <Button type="primary" @click="del()">确定</Button>
            <Button type="default" @click="delModal=false">取消</Button>
          </div>
        </Modal>
        <!-- 添加，编辑 -->
        <Modal v-model="operateModel" width="800" :styles="{top: '20px'}">
          <p slot="header">
            <span>{{operateFlage}}人员</span>
          </p>
          <div class="staff-form-group">
            <Form ref="staffInfo" :model="staffInfo" :rules="staffInfoValidate" :label-width="136">
              <div class="staff-form">
                <h4>个人基本信息</h4>
                <Row class="form" style="margin-bottom: 0;">
                  <Col :lg="12" :md="12" :sm="24">
                    <FormItem label="姓名" prop="name">
                      <Input type="text" v-model="staffInfo.name" placeholder="请输入姓名"></Input>
                    </FormItem>
                  </Col>
                  <Col :lg="12" :md="12" :sm="24">
                    <FormItem label="民族" prop="ethnic">
                      <Input type="text" v-model="staffInfo.ethnic" placeholder="请输入民族"></Input>
                    </FormItem>
                  </Col>
                </Row>
                <Row class="form" style="padding-top: 0;margin-bottom: 0;">
                  <Col :lg="12" :md="12" :sm="24">
                    <FormItem label="性别" prop="sex">
                      <RadioGroup v-model="staffInfo.sex">
                        <Radio :label="item.id" v-for="item in sSex" :key="item.id">{{item.name}}</Radio>
                      </RadioGroup>
                    </FormItem>
                  </Col>
                  <Col :lg="12" :md="12" :sm="24">
                    <FormItem label="年龄" prop="age">
                      <InputNumber
                        :max="120"
                        :min="1"
                        v-model="staffInfo.age"
                        :precision="0"
                        placeholder="请输入年龄"
                        style="width: 100%;"
                      ></InputNumber>
                    </FormItem>
                  </Col>
                </Row>
                <Row class="form" style="padding-top: 0;">
                  <Col :lg="12" :md="12" :sm="24">
                    <FormItem label="身份证号" prop="idcard">
                      <Input type="text" v-model="staffInfo.idcard" placeholder="请输入身份证号"></Input>
                    </FormItem>
                  </Col>
                  <Col :lg="12" :md="12" :sm="24">
                    <FormItem label="出生日期" prop="birthday">
                      <DatePicker
                        type="date"
                        format="yyyy-MM-dd"
                        style="width: 100%;"
                        placeholder="请选择出生日期"
                        v-model="staffInfo.birthday"
                      ></DatePicker>
                    </FormItem>
                  </Col>
                  <Col :lg="12" :md="12" :sm="24">
                    <FormItem label="联系电话" prop="phone">
                      <Input type="text" v-model="staffInfo.phone" placeholder="请输入联系电话"></Input>
                    </FormItem>
                  </Col>
                  <Col :lg="12" :md="12" :sm="24">
                    <FormItem label="紧急联系电话" prop="emergencyPhone">
                      <Input type="text" v-model="staffInfo.emergencyPhone" placeholder="请输入紧急联系电话"></Input>
                    </FormItem>
                  </Col>
                  <Col :lg="12" :md="24" :sm="24">
                    <FormItem label="户口所在地" prop="hjAddress">
                      <Input type="text" v-model="staffInfo.hjAddress" placeholder="请输入户口所在地"></Input>
                    </FormItem>
                  </Col>
                  <Col :lg="12" :md="24" :sm="24">
                    <FormItem label="现居住地" prop="contactAddress">
                      <Input type="text" v-model="staffInfo.contactAddress" placeholder="请输入现居住地"></Input>
                    </FormItem>
                  </Col>
                </Row>
              </div>

              <div class="staff-form">
                <h4>在职基本信息</h4>
                <Row class="form" style="margin-bottom: 0;">
                  <Col :lg="12" :md="12" :sm="24">
                    <FormItem label="部门" prop="depId">
                      <Select v-model="staffInfo.depId" filterable placeholder="请选择部门">
                        <Option
                          :value="item.id"
                          v-for="item in sDepartment"
                          :key="item.id"
                        >{{item.name}}</Option>
                      </Select>
                    </FormItem>
                  </Col>
                  <Col :lg="12" :md="12" :sm="24">
                    <FormItem label="职务" prop="positionId">
                      <Select v-model="staffInfo.positionId" filterable placeholder="请选择职位">
                        <Option
                          :value="item.id"
                          v-for="item in sPosition"
                          :key="item.id"
                        >{{item.name}}</Option>
                      </Select>
                    </FormItem>
                  </Col>
                    <Col :lg="12" :md="12" :sm="24">
                    <FormItem label="医师编码" prop="nationDoctCode">
                      <Input v-model="staffInfo.nationDoctCode" filterable placeholder="请输入国家医师编码"> </Input>
                    </FormItem>
                  </Col>
                  <Col :lg="12" :md="12" :sm="24">
                    <FormItem label="所在机构" prop="centerDialysisId">
                      <Select v-model="staffInfo.centerDialysisId" filterable placeholder="请选择所在机构">
                        <Option
                          :value="item.dialysisId"
                          v-for="item in hospitalList.filter(res => {
                            return res.dialysisId !== '0'
                          })"
                          :key="item.dialysisId"
                        >{{item.dialysisName}}</Option>
                      </Select>
                    </FormItem>
                  </Col>
                  <Col :lg="12" :md="12" :sm="24">
                    <FormItem label="在职情况" prop="workingState">
                      <Select v-model="staffInfo.workingState" placeholder="请选择在职情况">
                        <Option
                          :value="item.id"
                          v-for="item in sWorkingState"
                          :key="item.id"
                        >{{item.name}}</Option>
                      </Select>
                    </FormItem>
                  </Col>
                  <Col :lg="12" :md="12" :sm="24">
                    <FormItem label="入职日期" prop="hiredate">
                      <DatePicker
                        type="date"
                        format="yyyy-MM-dd"
                        style="width: 100%;"
                        placeholder="请选择入职日期"
                        v-model="staffInfo.hiredate"
                      ></DatePicker>
                    </FormItem>
                  </Col>
                  <Col :lg="12" :md="12" :sm="24">
                    <FormItem label="转正日期" prop="positiveDate">
                      <DatePicker
                        type="date"
                        format="yyyy-MM-dd"
                        style="width: 100%;"
                        placeholder="请选择转正日期"
                        v-model="staffInfo.positiveDate"
                      ></DatePicker>
                    </FormItem>
                  </Col>
                  <Col :lg="12" :md="12" :sm="24">
                    <FormItem label="合同签订日期" prop="signDate">
                      <DatePicker
                        type="date"
                        format="yyyy-MM-dd"
                        style="width: 100%;"
                        placeholder="请选择合同签订日期"
                        v-model="staffInfo.signDate"
                      ></DatePicker>
                    </FormItem>
                  </Col>
                  <Col :lg="12" :md="12" :sm="24">
                    <FormItem label="合同到期日期" prop="expireDAte">
                      <DatePicker
                        type="date"
                        format="yyyy-MM-dd"
                        style="width: 100%;"
                        placeholder="请选择合同到期日期"
                        v-model="staffInfo.expireDAte"
                      ></DatePicker>
                    </FormItem>
                  </Col>
                  <Col :lg="12" :md="12" :sm="24">
                    <FormItem label="外派合同情况" prop="assignmentStatus">
                      <Input type="text" v-model="staffInfo.assignmentStatus" placeholder="请输入"></Input>
                    </FormItem>
                  </Col>
                  <Col :lg="12" :md="12" :sm="24">
                    <FormItem label="户口性质" prop="householdRegister">
                      <Input type="text" v-model="staffInfo.householdRegister" placeholder="请输入"></Input>
                      <!-- <RadioGroup v-model="staffInfo.householdRegister">
                        <Radio label="城镇"></Radio>
                        <Radio label="农村"></Radio>
                      </RadioGroup>-->
                    </FormItem>
                  </Col>
                  <Col :lg="12" :md="12" :sm="24">
                    <FormItem label="劳动合同关系" prop="laborContract">
                      <Input type="text" v-model="staffInfo.laborContract" placeholder="请输入"></Input>
                    </FormItem>
                  </Col>
                  <Col :lg="12" :md="12" :sm="24">
                    <FormItem label="举荐人" prop="recommendations">
                      <Input type="text" v-model="staffInfo.recommendations" placeholder="请输入"></Input>
                    </FormItem>
                  </Col>
                  <Col :lg="12" :md="12" :sm="24">
                    <FormItem label="联系电话" prop="recommendationsPhone">
                      <Input
                        type="text"
                        v-model="staffInfo.recommendationsPhone"
                        placeholder="请输入举荐人联系电话"
                      ></Input>
                    </FormItem>
                  </Col>
                </Row>
              </div>

              <div class="staff-form">
                <h4>学历专业信息</h4>
                <Row class="form">
                  <Col :lg="12" :md="12" :sm="24">
                    <FormItem label="学历" prop="eduId">
                      <Select v-model="staffInfo.eduId" placeholder="请选择学历">
                        <Option
                          :value="item.id"
                          v-for="item in sEducation"
                          :key="item.id"
                        >{{item.name}}</Option>
                      </Select>
                    </FormItem>
                  </Col>
                  <Col :lg="12" :md="12" :sm="24">
                    <FormItem label="毕业院校（起点）" prop="startSchool">
                      <Input type="text" v-model="staffInfo.startSchool" placeholder="请输入学校"></Input>
                    </FormItem>
                  </Col>
                  <Col :lg="12" :md="12" :sm="24">
                    <FormItem label="毕业院校（最高）" prop="highestSchool">
                      <Input type="text" v-model="staffInfo.highestSchool" placeholder="请输入学校"></Input>
                    </FormItem>
                  </Col>
                  <Col :lg="12" :md="12" :sm="24">
                    <FormItem label="毕业院校(现有毕业证)" prop="graduateSchool">
                      <Input type="text" v-model="staffInfo.graduateSchool" placeholder="请输入学校"></Input>
                    </FormItem>
                  </Col>
                  <Col :lg="12" :md="12" :sm="24">
                    <FormItem label="专业" prop="major">
                      <Input type="text" v-model="staffInfo.major" placeholder="请输入专业"></Input>
                    </FormItem>
                  </Col>
                  <Col :lg="12" :md="12" :sm="24">
                    <FormItem label="毕业时间" prop="graduationDate">
                      <DatePicker
                        type="date"
                        format="yyyy-MM-dd"
                        style="width: 100%;"
                        placeholder="请选择毕业时间"
                        v-model="staffInfo.graduationDate"
                      ></DatePicker>
                    </FormItem>
                  </Col>
                  <Col :lg="12" :md="12" :sm="24">
                    <FormItem label="职称" prop="jobTitleId">
                      <Select v-model="staffInfo.jobTitleId" placeholder="请选择职称">
                        <Option
                          :value="item.id"
                          v-for="item in sJobTitle"
                          :key="item.id"
                        >{{item.name}}</Option>
                      </Select>
                    </FormItem>
                  </Col>
                </Row>
              </div>

              <div class="staff-form">
                <h4>工作经历</h4>
                <Row class="form">
                  <Col :lg="12" :md="12" :sm="12">
                    <FormItem label="血透经验年限" prop="jobYear">
                      <!-- <Input type="text" v-model="staffInfo.jobYear" placeholder="请输入"></Input> -->
                      <InputNumber
                        :max="120"
                        :min="1"
                        v-model="staffInfo.jobYear"
                        placeholder="请输入"
                        style="width: 100%;"
                      ></InputNumber>
                    </FormItem>
                  </Col>

                  <Col :lg="24" :md="24" :sm="24">
                    <FormItem label="工作经历" prop="experienceJob">
                      <Input
                        type="textarea"
                        :autosize="{minRows: 2,maxRows: 5}"
                        v-model="staffInfo.experienceJob"
                        placeholder="请输入工作经历"
                      ></Input>
                    </FormItem>
                  </Col>
                </Row>
              </div>
            </Form>
          </div>
          <div slot="footer" style="text-align: center">
            <Button type="primary" v-if="operateFlage=='添加'" @click="addSubmit('staffInfo')">确定</Button>
            <Button type="primary" v-else @click="editSubmit('staffInfo')">保存</Button>
            <Button type="default" @click="handleReset('staffInfo')">取消</Button>
          </div>
        </Modal>
      </div>
    </parent-view2>
  </div>
</template>

<script>
import swsUpload from '_c/sws-upload/'
import swsTable from '_c/sws-table/'
import Operate from '@/components/operate'
import staffInfoValidate from './staffInfoValidate.js'
import chartSetting from './chartSetting.js'
import staffDetail from './staff-detail.vue'
import parentView2 from '@/components/parent-view/parent-view2.vue'
import { getNowFormatDate } from '@/libs/tools'
import { setTimeCurrent } from '@/libs/util'
const [EDUCATION, DEPARTMENT, SEX, POSITION, JOBTITLE, WORKINGSTATE] = [
  'c3d21a1838634b3d845ab6a4418d9513',
  2,
  'bc26726c62364272acd4df12c61200c5',
  'cb30fac65abb48f1ba5b276d1fa76730',
  '74b0a4c9111f47d9a06223de0b35d1d8',
  '27ff3a8bfe044abcbd8cf0208654157f'
]
const BUTTONROLE = {
  ZGGL_XZRY: 'ZGGL_XZRY',
  ZGGL_DR: 'ZGGL_DR',
  ZGGL_DC: 'ZGGL_DC',
  ZGGL_SC: 'ZGGL_SC',
  ZGGL_XG: 'ZGGL_XG',
  ZGGL_XQ: 'ZGGL_XQ'
}
const IMPORTID = 1
export default {
  data () {
    return {
      empSelectType: 0,
      importId: IMPORTID,
      // 图表
      statisticTypeModel: 0,
      statisticType: [
        {
          type: 0,
          name: '性别'
        },
        {
          type: 1,
          name: '职称'
        },
        {
          type: 2,
          name: '职位'
        },
        {
          type: 3,
          name: '学历'
        }
      ],
      // 关键字搜索
      searchKey: '',
      // 表格头
      table_column: [],
      table_column_default: [
        {
          title: '姓名',
          key: 'name',
          minWidth: 65,
          render: (h, params) => {
            return h('div', [
              h('Icon', {
                props: {
                  type: 'person'
                }
              }),
              h('strong', params.row.name)
            ])
          }
        },
        {
          title: '国家医师编码',
          key: 'nationDoctCode',
          minWidth: 100,
          renderHeader: (h, params) => {
            return h('div', this.empSelectType!==2?'国家医师编码':'国家护士编码',)
          }
        },
        {
          title: '国家医师姓名',
          key: 'name',
          minWidth: 100,
          renderHeader: (h, params) => {
            return h('div', this.empSelectType!==2?'国家医师姓名':'国家护士姓名',)
          }
        },
        {
          title: '身份证',
          key: 'idcard',
          minWidth: 155
        },
        {
          title: '性别',
          key: 'sex',
          width: 65,
          render: (h, params) => {
            let sex = params.row.sex
            let text
            text = this.sSex.filter(v => {
              return v.id === sex
            })[0].name
            return <span>{{ text }}</span>
          }
        },
        {
          title: '年龄',
          key: 'age',
          width: 65
        },
        {
          title: '部门',
          key: 'department',
          minWidth: 75
        },
        {
          title: '职位',
          minWidth: 75,
          key: 'position'
        },
        {
          title: '学历',
          key: 'education',
          width: 65
        },
        {
          title: '入职时间',
          key: 'hiredate',
          width: 100
        },
        {
          title: '在职状态',
          key: 'strWorkingState',
          align: 'center',
          width: 100
        },
        {
          title: '操作',
          key: 'action',
          width: 150,
          align: 'center',
          render: (h, params) => {
            return (
              <Operate
                handleEdit={() => this.handleEdit(params.row)}
                handleWatch={() => {
                  let staffDetail = this.staff_data.filter(v => {
                    return v.id === params.row.id
                  })[0]
                  this.staffDetail = staffDetail
                  this.$refs.staffDetail.show()
                }}
                handleDelete={() => {
                  this.delModal = true
                  this.tableRowIndex = params.row.id
                }}
                permissionDelete={this.buttonRole.ZGGL_SC}
                permissionEdit={this.buttonRole.ZGGL_XG}
              />
            )
          }
        }
      ],
      staff_data: [],
      staffDataCount: 0,
      startPage: 1,
      // 删除模态框
      delModal: false,
      // 员工添加or编辑模态框
      operateModel: false,
      // 开启添加or编辑
      operateFlage: 'add',

      tableRowIndex: -1,
      loading: false,
      pageSize: 7,
      // 透析机构列表
      hospitalCheckedId: '0',
      hospitalCheckedName: '',
      hospitalList: [],

      // 表单
      staffInfo: {
        // 基本信息
        name: '',
        sex: '',
        ethnic: '',
        idcard: '',
        birthday: '',
        workingState: '',
        age: 0,
        phone: '',
        hjAddress: '',
        emergencyPhone: '',
        contactAddress: '',
        department: '',
        depId: '',
        position: '',
        positionId: '',
        hiredate: '',
        positiveDate: '',
        signDate: '',
        expireDAte: '',
        assignmentStatus: '',
        householdRegister: '',
        laborContract: '',
        recommendations: '',
        recommendationsPhone: '',
        education: '',
        eduId: '',
        startSchool: '',
        highestSchool: '',
        graduateSchool: '',
        educationBackground: '',
        major: '',
        graduationDate: '',
        jobTitleId: '',
        jobYear: 0,
        experienceJob: '',
        experienceJobTwo: '',
        founder: '',
        founderDate: '',
        modifier: '',
        modifierDate: '',
        dataState: 0,
        centerDialysisId: ''
      },

      staffInfoValidate: staffInfoValidate,
      staffDetail: {}, // 员工详情

      // 数据字典
      sEducation: [],
      sJobTitle: [],
      sDepartment: [],
      sSex: [],
      sPosition: [],
      sWorkingState: [],

      buttonRole: BUTTONROLE
    }
  },
  mixins: [chartSetting],
  created () {
    let types = [EDUCATION, DEPARTMENT, SEX, POSITION, JOBTITLE, WORKINGSTATE]
    types = types.map(id => {
      let o = {
        url: 'SystemDictionary/DictionaryList',
        params: { typeId: `${id}` }
      }
      return o
    })
    this.swsApi
      .swsAllPost(types)
      .then(res => {
        this.sEducation = res[0].data.result
        this.sDepartment = res[1].data.result
        this.sSex = res[2].data.result
        this.sPosition = res[3].data.result
        this.sJobTitle = res[4].data.result
        this.sWorkingState = res[5].data.result
      })
      .catch(e => {
        console.log(e)
      })
  },
  computed: {
    chartTitle () {
      let name = null
      if (this.statisticTypeModel === 0) {
        // 按性别
        name = '性别'
      } else if (this.statisticTypeModel === 1) {
        // 按职称
        name = '职称'
      } else if (this.statisticTypeModel === 2) {
        // 按职位
        name = '职位'
      } else if (this.statisticTypeModel === 3) {
        // 按学历
        name = '学历'
      } else return false

      return `${this.hospitalCheckedName}医护人员${name}比例统计`
    },
    chartApi () {
      let api = null
      if (this.statisticTypeModel === 0) {
        // 按性别
        api = 'Employee/GetEmplyeeBySexList/'
      } else if (this.statisticTypeModel === 1) {
        // 按职称
        api = 'Employee/GetEmplyeeByJobTitleList/'
      } else if (this.statisticTypeModel === 2) {
        // 按职位
        api = 'Employee/GetEmplyeeByProfessionList/'
      } else if (this.statisticTypeModel === 3) {
        // 按学历
        api = 'Employee/GetEmplyeeByEducationxList/'
      } else return false

      return api + this.hospitalCheckedId
    },
    pagination () {
      return { total: this.staffDataCount, current: this.startPage, pageSize: this.pageSize }
    }
  },
  methods: {
    empTypeChange () {
      this.getStaffList(this.startPage)
    },
    // 离职员工行显示灰色
    rowClassName (row, index) {
      // 离职typeID：e81a52d7e88e49a28dd536548ca8fe8a
      if (row.workingState === 'e81a52d7e88e49a28dd536548ca8fe8a') {
        return 'resigned'
      }
      return ''
    },
    // 分页
    changePage (index) {
      this.startPage = index
      this.getStaffList(index)
    },
    // 表单验证
    addSubmit (name) {
      this.staffInfo.founder = sessionStorage.getItem('username')
      this.staffInfo.founderDate = getNowFormatDate()
      this.staffInfo.id && delete this.staffInfo.id
      this.handldSubmit(name, this.staffInfo, '添加')
    },
    editSubmit (name) {
      this.staffInfo.modifier = sessionStorage.getItem('username')
      this.staffInfo.modifierDate = getNowFormatDate()
      this.handldSubmit(name, this.staffInfo, '修改')
    },
    handldSubmit (name, obj, str) {
      this.$refs[name].validate(valid => {
        if (valid) {
          this.loading = true
          obj.birthday = obj.birthday ? setTimeCurrent(obj.birthday) : ''
          obj.hiredate = obj.hiredate ? setTimeCurrent(obj.hiredate) : ''
          obj.positiveDate = obj.positiveDate
            ? setTimeCurrent(obj.positiveDate)
            : ''
          obj.signDate = obj.signDate ? setTimeCurrent(obj.signDate) : ''
          obj.expireDAte = obj.expireDAte ? setTimeCurrent(obj.expireDAte) : ''
          obj.graduationDate = obj.graduationDate
            ? setTimeCurrent(obj.graduationDate)
            : ''
          this.swsApi
            .swsPost('Employee/CreateUpdateEmployee', obj)
            .then(res => {
              this.loading = false
              if (res.data.result) {
                this.operateModel = false
                this.$Message.success(`${str}成功`)

                this.$refs.tree.getHospital()
                // 重新获取数据
                this.getStaffList()
                // 设置图表
                this.setPieChart(this.chartApi)
              } else {
                this.$Message.info('操作失败，请稍后再试')
              }
            })
            .catch(e => {
              this.loading = false
            })
        } else {
          this.$Message.error('请检查是否输入完成')
        }
      })
    },
    handleReset (name) {
      this.operateModel = false
      this.$refs[name].resetFields()
    },
    add () {
      this.$refs['staffInfo'].resetFields()
      this.operateFlage = '添加'
      this.operateModel = true
    },
    del () {
      this.delModal = false
      this.swsApi.swsGet(`Employee/del/${this.tableRowIndex}`).then(res => {
        if (res.data.result) {
          this.$Message.success('删除成功！')
          this.startPage = 1
          this.$refs.tree.getHospital()
        } else {
          this.$Message.error('操作失败，请稍后再试')
        }
      })
    },
    // 获取员工列表
    getStaffList (i = 1) {
      let args = {
        dialysisId: this.hospitalCheckedId,
        pageSize: this.pageSize,
        empName: this.searchKey,
        pageNum: i,
        isQureydoctor: this.empSelectType
      }
      this.loading = true
      this.swsApi
        .swsPost('Employee/Employee', args)
        .then(res => {
          this.loading = false
          if (res.data.result) {
            this.staff_data = res.data.result
            this.staffDataCount = res.data.dataCount
          } else {
            this.$Message.error('网络错误，请稍后再试')
          }
        })
        .catch(e => {
          console.log(e)
          this.loading = false
        })
    },
    // 切换医院
    changeHospital (id, list) {
      this.searchKey = ''
      list.length && (this.hospitalList = list)
      this.hospitalCheckedId = id
      this.hospitalCheckedName = this.hospitalList.filter(
        v => v.dialysisId === id
      )[0].dialysisName
      // 表格-》部门是否展示
      this.table_column =
        this.hospitalCheckedId === '0'
          ? this.table_column_default
          : this.table_column_default.filter(res=>res.title!=='部门')
      // 设置图表
      this.setPieChart(this.chartApi)
      this.startPage = 1
      this.getStaffList(this.startPage)
      sessionStorage.setItem('hospitalCheckedId', id)
    },
    // 搜索
    searchStaff () {
      this.startPage = 1
      this.getStaffList(this.startPage)
    },
    // 导出图表
    download () {
      let args = {
        dialysisId: this.hospitalCheckedId,
        pageSize: 10000,
        empName: '',
        pageNum: 1
      }
      this.swsApi
        .swsPost('Employee/Employee', args)
        .then(res => {
          if (res.data.result) {
            let data = res.data.result.map(item => {
              let sexId = item.sex
              item.sex = this.sSex.filter(v => {
                return v.id === sexId
              })[0].name
              return item
            })
            // console.log(this.$refs.table)
            this.$refs.table.exportCsv({
              filename: `${this.hospitalCheckedName}医护人员统计表`,
              columns: this.table_column.slice(0, -1),
              data
            })
          } else {
            this.$Message.error('网络错误，请稍后再试')
          }
        })
        .catch(e => {
          console.log(e)
          this.loading = false
        })
    },
    //
    changeChartType () {
      // 设置图表
      this.setPieChart(this.chartApi)
    },
    handleUploadSuccess () {
      this.$refs.tree.getHospital()
    },
    handleEdit ({ id }) {
      this.handleReset('staffInfo')
      let staffInfo = this.staff_data.filter(v => {
        return v.id === id
      })[0]

      this.staffInfo = Object.assign(this.staffInfo, staffInfo)
      this.staffInfo.birthday = staffInfo.birthday
        ? new Date(staffInfo.birthday)
        : null
      this.staffInfo.positiveDate = staffInfo.positiveDate
        ? new Date(staffInfo.positiveDate)
        : null
      this.staffInfo.hiredate = staffInfo.hiredate
        ? new Date(staffInfo.hiredate)
        : null
      this.staffInfo.signDate = staffInfo.signDate
        ? new Date(staffInfo.signDate)
        : null
      this.staffInfo.expireDAte = staffInfo.expireDAte
        ? new Date(staffInfo.expireDAte)
        : null
      delete this.staffInfo.expireDate
      this.operateFlage = '编辑'
      this.operateModel = true
    }
  },
  components: {
    staffDetail,
    parentView2,
    swsUpload,
    Operate,
    swsTable
  }
}
</script>

<style scoped lang="less">
// .fade-enter-active,
// .fade-leave-active {
//   transition: opacity 0.05s;
// }
// .fade-enter, .fade-leave-to /* .fade-leave-active below version 2.1.8 */ {
//   opacity: 0;
// }
#staff {
  position: relative;
  width: 100%;
  height: 100%;
  .chart {
    height: 350px;
    padding: 20px;
    background: #ffffff;
    position: relative;
    .btn-group {
      position: absolute;
      right: 40px;
      z-index: 10;
      /deep/ .ivu-btn-primary {
        background: #4f95e8;
        border-color: #4f95e8;
        box-shadow: 2px 2px 6px rgba(79, 149, 232, 0.35);
      }
    }
  }
}
.staff-operate {
  margin-top: 20px;
  padding: 20px 0;
  background: #ffffff;
  .operate {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 0 20px;
  }
  // cy:调大table的字体
  .sws-table /deep/ .ivu-table-wrapper {
    & /deep/ .ivu-table {
      td {
        font-size: 13px;
      }
      .resigned {
        color: #999;
      }
    }
  }
}
.staff-form-group {
  .staff-form {
    h4 {
      margin-bottom: 10px;
      padding-left: 10px;
      font-size: 16px;
    }
    .form {
      padding-right: 20px;
    }
  }
}
#staff-detail {
  position: absolute;
  top: 0;
  right: 0;
  width: 500px;
  height: 100%;
  z-index: 10;
}
</style>
