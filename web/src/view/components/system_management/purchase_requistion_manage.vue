<template>
  <div id="menu-manage" class="container">
    <Spin fix v-if="spinShow">
      <Icon type="ios-loading" class="demo-spin-icon-load" size="30"></Icon>
      <div class="text-loading">加载中</div>
    </Spin>
    <Row class="box">
      <i-col class="menu box-left" :sm="10" :md="8" :lg="5">
        <div class="top">
          <div class="top_left">
            <span style="color: #999999; font-size: 15px; margin-right: 22px;">配置目录</span>
          </div>
        </div>
        <div class="menuTree">
          <Tree ref="treeRef" :data="treeData" @on-select-change="handleClickTreeNode" style="margin-left: 12px;"></Tree>
        </div>
      </i-col>
      <i-col class="operate-box box-right" :sm="14" :md="16" :lg="19">
        <!-- 详情-修改-删除 -->
        <div>
          <div class="top">
            <div class="top_right">{{title}} 审批配置
						</div>
          </div>
          <div class="edit-box" style="position: relative;">
						<Spin fix v-if="!tpyeId">
							<Icon type="ios-loading" class="demo-spin-icon-load" size="30"></Icon>
							<div class="text-loading">请选择目录</div>
						</Spin>
            <Form
              ref="formValidateRef"
              class="form"
              :model="formValidate"
              :label-width="110"
              label-position="right"
            >
							<Row v-for="(item,index) in formValidate.items" :key="index">
								<Col span="24">
									<FormItem
									:prop="'items.' + index + '.medicalItemTypeArr'"
									:label="'审批类型 '"
									:rules="ruleValidate.medicalItemTypeArr">
										<Select v-model="item.medicalItemTypeArr" filterable multiple placeholder>
											<Option v-for="item in MedicalItemList" :key="item.type" :value="item.type">{{item.title}}</Option>
										</Select>
									</FormItem>
								</Col>
								<Col span="7">
									<FormItem
									:prop="'items.' + index + '.purchSubmitLevel'"
									:label="'审批层级 ' + item.purchSubmitLevel">
										<InputNumber style="width:90px;" v-model="item.purchSubmitLevel" placeholder="审批层级"></InputNumber>
									</FormItem>
								</Col>
								<Col span="7">
									<FormItem
									:prop="'items.' + index + '.userId'"
									:label="'人员 ' + item.purchSubmitLevel"
									:rules="{required: true, message: '人员 ' + item.purchSubmitLevel +' 不能为空', trigger: 'change'}">
											<Select v-model="item.userId" @on-change="getUserInfo($event,item,index)" filterable placeholder style="width:90px;">
												<Option v-for="item in userList" :key="item.id" :value="item.id">{{item.name}}</Option>
												<!-- <Option v-for="item in userList" :key="item.id" :value="item.id" :label="item.position">
													<div slot>{{item.name}}</div>
												</Option> -->
											</Select>
									</FormItem>
								</Col>
								<Col span="10">
									<FormItem
									:prop="'items.' + index + '.userRemark'"
									:label="'职位 ' + item.purchSubmitLevel">
										<Input type="text" v-model="item.userRemark" placeholder="人员职位" ></Input>
									</FormItem>
								</Col>
								<Col span="7"  v-if="tpyeId=='2'">
									<FormItem
									:prop="'items.' + index + '.minAmount'"
									:label="'审核金额下限 ' + item.purchSubmitLevel">
										<InputNumber style="width:90px;" type="text" v-model="item.minAmount" placeholder="审核金额下限"></InputNumber>
									</FormItem>
								</Col>
								<Col span="7" :offset="7" v-if="tpyeId=='2'">
									<FormItem
									:prop="'items.' + index + '.maxAmount'"
									:label="'审核金额上限 ' + item.purchSubmitLevel">
										<InputNumber style="width:90px;" type="text" v-model="item.maxAmount" placeholder="审核金额上限"></InputNumber>
									</FormItem>
								</Col>
								<Col span="21">
									<FormItem 
									:prop="'items.' + index + '.describe'"
									:label="'备注 ' + item.purchSubmitLevel">
										<Input
											v-model="item.describe"
											type="textarea"
											:rows="2"
											placeholder="请输入备注信息"
										/>
									</FormItem>
								</Col>
								<Col span="3" v-show="formValidate.items.length>1"><Button style="margin-left:29px;" type="error" @click="handleRemove(item.purchSubmitLevel)">删除</Button></Col>
								<Divider/>
							</Row>
							<FormItem>
								<Button type="dashed" long @click="AddItem" icon="md-add">新增审批层级</Button>
							</FormItem>
							<FormItem>
									<Button :loading="btnLoading" type="success" v-if="addFlag" v-permission="buttonRole.CGSPPZ_ADD" @click="handleSubmit('formValidateRef')">新增</Button>
									<Button :loading="btnLoading" type="warning" v-else v-permission="buttonRole.CGSPPZ_EDIT" @click="handleSubmit('formValidateRef')">修改</Button>
									<Button :loading="btnLoading" @click="handleReset('formValidateRef')" style="margin: 0 8px">清空</Button>
									<Button v-show="!addFlag" @click="getPreviewData" type="primary" style="float: right;" :loading="previewLoading">预览</Button>
							</FormItem>
            </Form>
          </div>
        </div>
      </i-col>
    </Row>
    <Drawer :title="`${title}审批流程预览`" :closable="false"  v-model="showPreviewFlag">
			<Steps :current="currentStep" direction="vertical">
				<Step
					v-for="(item, index) in orderApproveOutPuts"
					:key="index"
					:title="item.title"
					:content="`${item.content}${item.time ? '，时间：': ''}${item.time ? item.time : ''}`"
				></Step>
			</Steps>
			<!-- <div slot="footer">
				<Button @click="showPreviewFlag=false;">关闭</Button>
			</div> -->
    </Drawer>
  </div>
</template>

<script>
const BUTTONROLE = {
  CGSPPZ_ADD: 'CGSPPZ_ADD',
  CGSPPZ_EDIT: 'CGSPPZ_EDIT',
}
export default {
  name: 'purchaseRequistionManage',
  data () {
    return {
			orderApproveOutPuts: [],
			showPreviewFlag: false,
			index: 1,
			userList: [],//员工列表
			MedicalItemList: [
        {title:'药品',type:'1'},
        {title:'耗材',type:'2'},
        {title:'固定资产',type:'3'},
        {title:'低值易耗品',type:'4'},
			],
      treeData: [
        {title:'采购申请',id:'1'},
        {title:'采购订单',id:'2'},
      ],
      addFlag: true,
      spinShow: false,
      tpyeId: '',
			items:{
				medicalItemTypeArr: [],
				purchSubmitLevel: 1,
				userRemark: '',
				userId: '',
				userName: '',
				minAmount: 0,
				maxAmount: 100,
				describe: ''
			},
      formValidate: {
				items: [],
				// medicalItemTypeArr: [],
      },
      ruleValidate: {
        medicalItemTypeArr: [
          {
            required: true,
            type: 'array',
            message: '类型不能为空',
            trigger: 'blur'
          },
          { type: 'array', message: '类型不能为空', trigger: 'change' }
        ],
      },
      buttonRole: BUTTONROLE,
			btnLoading: false,
			previewLoading: false
    }
  },
	
  mounted () {
		this.initForm()
		this.getUserListData()
		// this.handleClickTreeNode([this.treeData[0]])
		// let aaa = this.$refs.treeRef.getSelectedNodes()
		// console.log(aaa);
  },
	computed: {
		title(){
			if(this.tpyeId)
				return this.treeData.find(res=>res.id==this.tpyeId).title
			else return ''
		},
    currentStep () {
      if (this.orderApproveOutPuts.length > 0) {
        return this.orderApproveOutPuts[0].current
      } else {
        return 0
      }
    },
    currentStatus () {
      if (1) {
        return 'error'
      } else {
        return 'process'
      }
    },
	},
  methods: {
		getUserInfo(id,item,index){
			// console.log(id,item);
			let position = this.userList.find(res=>res.id==id).position
			// let i = this.findObjIndex(this.formValidate.items,'purchSubmitLevel',item.purchSubmitLevel)
			if(index>=0) {
				// this.formValidate.items[i].userRemark = position
				this.formValidate.items[index].userRemark = position
			}
		},
		initForm(){
			this.formValidate.items = [{...this.items}]
		},
		handleSubmit (name) {
			if(!this.tpyeId) {
				this.$Message.warning('请选择目录！');
				return false
			}
			this.$refs[name].validate((valid) => {
				if (valid) {
					let list = [...this.formValidate.items]
					let levelSet = new Set()
					let userSet = new Set()
					let json = list.map(res=>{
						levelSet.add(res.purchSubmitLevel)
						userSet.add(res.userId)
						res.medicalItemType = res.medicalItemTypeArr.join(',')
						res.approvalType = this.tpyeId
						if(this.tpyeId=='1') {
							delete res.minAmount
							delete res.maxAmount
						}
						return res
					})
					// if(levelSet.size!=list.length) {
					// 	this.$Message.warning('审核层级不能重叠！');
					// 	return false
					// } 
					// if (userSet.size!=list.length) {
					// 	this.$Message.warning('审核人员不能重复！');
					// 	return false
					// }
					// console.log(json);
					this.swsApi.swsPost('CenterDocking/Purchase/AddApprovalConfig',json).then(res=>{
						this.spinShow = true
						if(res.data.code==200){
							this.spinShow = false
							this.$Message.success('操作成功！');
							this.handleClickTreeNode([this.treeData[parseInt(this.tpyeId)-1]])
						}
					})
					.catch(e=>{
						this.$Message.warning('请求异常',e);
						this.spinShow = false
					})
				} else {
					this.$Message.error('请检查填写项目数据。');
				}
			})
		},
		handleReset (name) {
			this.$refs[name].resetFields();
		},
		AddItem () {
			this.index = this.formValidate.items.length + 1
			let data = {...this.items}
			data.purchSubmitLevel= this.index
			this.formValidate.items.push(data);
		},
		findObjIndex(obj,key,id){
      let findCode = (el)=> el[key]==id
			return obj.findIndex(findCode)
		},
		handleRemove (purchSubmitLevel) {
			let index = this.findObjIndex(this.formValidate.items,'purchSubmitLevel',purchSubmitLevel)
			if (index>=0) this.formValidate.items.splice(index,1)
		},
		getPreviewData(){
			this.previewLoading=true
      this.swsApi.swsGet(`CenterDocking/Purchase/ApprovalConfigPreview/${this.tpyeId}`).then(res => {
        // console.log(res)
				let da = res.data
        if (da.result.length) {
					this.orderApproveOutPuts = da.result
					this.showPreviewFlag = true
        } else {
					this.$Message.warning('暂无数据');
				}
				this.previewLoading=false
      })
			.catch(e=>{
				this.$Message.warning('请求异常',e);
				this.previewLoading=false
				this.spinShow = false
			})
		},
    // 点击菜单项
    handleClickTreeNode (e) {
			// console.log(e);
      if (!e.length) return
      this.tpyeId = e[0].id
			this.btnLoading = true
      this.swsApi.swsGet(`CenterDocking/Purchase/GetApprovalConfig/${this.tpyeId}`).then(res => {
        // console.log(res)
				let da = res.data
        if (da.result.length) {
					this.addFlag = false
					this.formValidate = {
						// medicalItemTypeArr: da.result[0].medicalItemType.split(','),
						items: da.result.map(res=>{
							res.medicalItemTypeArr = res.medicalItemType.split(',')
							return res
						})
					}
        } else {
					this.addFlag = true
					this.handleReset('formValidateRef')
					this.initForm()
				}
				this.btnLoading = false
				this.spinShow = false
      })
			.catch(e=>{
				this.$Message.warning('请求异常',e);
				// console.log(e);
				this.btnLoading = false
				this.spinShow = false
			})
    },
    // 获取用户列表
    getUserListData () {
      this.swsApi.swsPost('User/userlist',{pageNum: 1,pageSize: 9999}).then(res => {
        let da = res.data
				if(da.code==200&&da.result.length){
					this.userList = da.result.filter(res=>res.isActive==1)
				}
      })
    }
  }
}
</script>

<style scoped lang='less'>
#menu-manage {
  background-color: #fff;
  height: 100%;
  position: relative;
  overflow: hidden;
  .menu,
  .operate-box {
    height: 100%;
  }
  .operate-box {
    .top + div {
      padding: 20px;
    }
    & > div {
      height: 100%;
      display: flex;
      flex-direction: column;
    }
  }
  .form {
    max-width: 680px;
  }
}
/deep/ .menuTree .ivu-tree .ivu-tree-children li .ivu-tree-title {
  font-size: 15px;
  color: #424242;
}

.container {
  background-color: #fff;
  height: 100%;
  position: relative;
  .box {
    height: 100%;
    & > * {
      height: 100%;
      display: flex;
      flex-direction: column;
      .top {
        width: 100%;
        height: 50px;
        line-height: 50px;
        border-bottom: 1px solid #eaeaea;
        .top_left {
          padding-left: 20px;
        }
        .top_right {
          color: #999999;
          font-size: 15px;
          padding-left: 20px;
        }
        & + * {
          padding-top: 20px;
          flex: 1;
          overflow-y: auto;
        }
      }
    }
    &-left {
      height: 100%;
      border-right: 1px solid #eaeaea;
      .menuTree {
        margin-left: 30px;
      }
    }
  }
}
</style>
