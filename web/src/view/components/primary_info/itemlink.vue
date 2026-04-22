<template>
	<div>
		<Card>
			<Button icon="ios-search" style="margin-right:10px;" type="primary" @click="getData">查询</Button>
			<Button icon="ios-link" type="success" @click="addLinkModal=true;">添加关联</Button>
			<!-- <Input v-model="searchKey" search enter-button placeholder="搜索..." @on-search="getData" style="width: 240px;display: inline-table;margin-left:10px;"/> -->
		</Card>
		<Table ref="tableA" :loading="tableLoading" highlight-row @on-row-click="setTableBData" :columns="columnsA" :data="tableDataA" :height="tableHeight">
		</Table>
		<div class="pagination">
			<Button style="float:left;" type="warning" v-show="!cancelLinkFlag" @click="cancelLinkToggle">取消关联</Button>
			<Button style="float:left;margin-right:10px;" type="success" v-show="cancelLinkFlag" @click="handleLink(false)" :loading="linkLoading">确定</Button>
			<Button style="float:left;" type="default" v-show="cancelLinkFlag" @click="cancelLinkToggle" :loading="linkLoading">取消</Button>
			<Page
				:total="dataCountA"
				:page-size="pageSize"
				:current.sync="startPageA"
				@on-change="handleChangePageA"
			/>
		</div>
		<Table ref="tableB" :loading="tableLoading" :columns="columnsB" :data="tableDataB" :height="tableHeight" @on-selection-change="selectItemChange">
		</Table>
		<!-- <div class="pagination">
			<Page
				:total="dataCountB"
				:page-size="pageSize"
				:current.sync="startPageB"
				@on-change="handleChangePageB"
			/>
		</div> -->
	<Modal v-model="addLinkModal" width="1010" @on-cancel="cancel">
		<p slot="header">
			<span>新增关联</span>
		</p>
		<div>
			类型:
      <Select
        style="width: 100px;"
        placeholder="请选择类型"
        v-model="selectedType"
        @on-change="getItemList">
        <Option :value="item.type" v-for="(item,index) in typeList" :key="index">{{item.name}}</Option>
      </Select>
			&nbsp;&nbsp;主物品:
      <Select
        style="width: 460px;"
        placeholder="搜索主物品..." @on-change="initSelect" @on-clear="initSelect"
				filterable clearable :loading="itemLoading"
        v-model="selectedItemId">
        <Option style="width:790px;" :value="item.id" v-for="(item,index) in itemList" :key="index">{{item.medicalItemName}} {{item.manufacturer}} {{item.medicalItemType==1?item.isSplited?item.minDose:item.packaging :item.minDose}}</Option>
      </Select>
			&nbsp;&nbsp;关联物品:
			<Input v-model="searchItemKey" placeholder="搜索名称/厂家..."  style="width: 140px;display: inline-table;margin-right:10px;"/>
			<Button type="primary" @click="showChange">{{showSelectedItem?'显示物品':'显示已选'}}</Button>
			<Table v-show="!showSelectedItem" ref="tableItem" :loading="itemLoading" :columns="columnsD" :data="computedItemData"
				@on-select="selectItem"
				@on-select-cancel="selectItemCancel"
				:height="500" class="" style="margin-top:10px;">
			</Table>
			<Table v-show="showSelectedItem" ref="tableItemSelected" :columns="columnsC" :data="computedItemDataSelect"
				:height="500" class="" style="margin-top:10px;">
          <template slot="action" slot-scope="{row}">
						<Tooltip content="取消选择"  transfer>
							<Icon type="md-close" size="18" style="cursor:pointer;" color="red" @click="selectItemCancel([],row)"/>
						</Tooltip>
          </template>
			</Table>
		</div>
		<div slot="footer">
			<span style="float: left;">已选择: {{selectionListArr.length}} 项</span>
			<Button @click="cancel">关闭</Button>
			<Button type="primary" @click="handleLink" :loading="linkLoading">确定关联</Button>
		</div>
	</Modal>
	</div>
</template>
<script>
import { toFilterKey,deepClone } from '@/libs/tools'
export default {
	name: 'itemlink',
	data() {
		return {
			cancelLinkFlag: false,
			linkLoading: false,
			showSelectedItem: false,
			selectionList: new Set(),
			selectionListArr: [],
			itemList: [],
			selectedType: 1,
			selectedItemId: '',
			addLinkModal: false,
			searchKey: '',
			searchItemKey: '',
			tableHeight: 0,
			typeList: [
				{type:1,name:'药品'},
				{type:2,name:'卫生耗材'},
				{type:4,name:'低值易耗'},
			],
			tableDataA: [],
			columnsA: [
        { title: "名称",key: "medicalItemName",minWidth: 200 },
        { title: "商品名",key: "brand",minWidth: 200 },
        { title: "编码",key: "medicalItemCode",minWidth: 120 },
        { title: "规格",key: "packaging",minWidth: 100 },
        { title: "厂家",key: "manufacturer",minWidth: 300 },
			],
			dataCountA: 0,
			startPageA: 1,
			tableDataB: [],
			columnsB: [],
			columnsD: [
				{ title: '', type: 'selection',width:55},
        { title: "名称",key: "medicalItemName",width:180},
        { title: "商品名",key: "brand",width:100},
        { title: "编码",key: "medicalItemCode",width:120},
        { title: "规格",key: "isSplited",width:120,
          render: (h, params) => {
						let da = params.row.medicalItemType==1?params.row.isSplited?params.row.minDose:params.row.packaging :params.row.minDose || '/'
            return  <span>{da}</span>
          }
				},
        { title: "厂家",key: "manufacturer",width:385},
			],
			columnsC: [
        { title: "名称",key: "medicalItemName",width:180},
        { title: "商品名",key: "brand",width:100},
        { title: "编码",key: "medicalItemCode",width:120},
        { title: "规格",key: "packaging",width:120},
        { title: "厂家",key: "manufacturer",width:340},
        { title: "操作",slot: "action",width:100,
				},
			],
			dataCountB: 0,
			startPageB: 1,
			pageSize: 10,
			itemLoading: false,
			tableLoading: false,
			highLightRow: {},
			selectCancelList: []
		}
	},
	created () {
		this.columnsB = deepClone(this.columnsA)
		this.getItemList()
		this.getData()
	},
  mounted () {
    // cy 调整table高度
    this.tableHeight = (document.documentElement.clientHeight - 300)/2
  },
	computed: {
		computedItemData(){
			let data = this.itemList
			if(this.selectedItemId)  data = this.itemList.filter(res=> res.id!==this.selectedItemId)
			if(this.searchItemKey){
				data = toFilterKey(data,"medicalItemName,brand,manufacturer",this.searchItemKey);
			}
			return data
		},
		computedItemDataSelect () {
			let	data = []
			if(this.showSelectedItem)
				data = this.itemList.filter(res=> this.selectionListArr.includes(res.id))
			return data
		}
	},
	methods:{
		cancel(){
			this.selectedItemId = ''
			this.addLinkModal = false
			this.showSelectedItem = false;
			this.initSelect()
		},
		initSelect(){
			this.searchItemKey = ''
			this.selectionList.clear()
			this.selectionListArr = []
		},
		showChange(){
			this.showSelectedItem=!this.showSelectedItem;
			if(!this.showSelectedItem) this.setChecked()
		},
		cancelLinkToggle(){
			if(!this.cancelLinkFlag) {
				this.columnsB.unshift({
					type: 'selection',
					width: 55,
					align: 'center'
				})
			} else {
				this.columnsB.shift()
			}
			this.cancelLinkFlag = !this.cancelLinkFlag;
		},
		handleLink(action=true){
			// console.log(action);
			if((!this.selectedItemId&&action)||(this.highLightRow.mainMedId==""&&!action)){
				this.$Message.warning("请选择主物品！");
				return false
			} else if((this.selectionListArr.length==0&&action)||(this.selectCancelList.length==0&&!action)){
				this.$Message.warning("请至少选择一项关联物品！");
				return false
			}
			let json = []
			if(action){
				json = this.selectionListArr.map(res=> {return {'mainMedId':this.selectedItemId,'childMedId':res}})
			} else {
				json = this.selectCancelList.map(res=> {return {'mainMedId':this.highLightRow.mainMedId,'childMedId':res.id}})
			}
			this.linkLoading = true;
			this.swsApi.swsPost(`Data/MRelevancy/${action?'Create':'Delete'}`,json).then((response)=>{
				// console.log(response);
				if(response.data.code==200){
					this.$Message.success("请求成功！");
					if(action) this.cancel()
					this.getData()
				}else{
					this.$Message.warning("请求失败！");
				}
				this.linkLoading = false;
			})
		},
		getData(){
      let params = {
        medicalItemType: this.selectedType,
        pageSize: 99999,
        pageNum: 1,
      }
      this.tableLoading = true
      this.swsApi.swsPost('Data/MRelevancy/GetALL').then(res => {
        if (res.data.success) {
          this.tableLoading = false
					// console.log(res);
					this.tableDataA = res.data.result.map(res=>{
						res = {...res.mainMedModel,...res}
						return res
					})
					this.setTableBData(this.tableDataA[0])
        }
      })
		},
		setTableBData(row,index){
			this.tableDataB = row.childMedModel
			this.highLightRow = row
		},
		selectItem(selection,row){
			if(!this.selectedItemId) {
				this.$Message.warning("请先选择主物品！");
				return false
			}
			this.selectionList.add(row.id)
			this.selectionListArr = Array.from(this.selectionList)
		},
		selectItemCancel(selection,row){
			this.selectionList.delete(row.id)
			this.selectionListArr = Array.from(this.selectionList)
		},
		selectItemChange(selection){
			this.selectCancelList = selection
		},
		search(){

		},
		handleChangePageA(){

		},
		handleChangePageB(){

		},
    // cy 给跨页丢失的选中行重新添加选中/禁用状态
    setChecked () {
      // 当前页的table数据
			// console.log('setChecked');
      let objData = this.$refs.tableItem.objData
			// console.log('setChecked',objData);
      for (let index in objData) {
        // 初始化禁用、已勾选状态
        // objData[index]._isDisabled = false
        objData[index]._isChecked = false
        // cy 根据保存的已勾选id来设置勾选状态
        if (this.selectionList.has(objData[index].id)) {
          objData[index]._isChecked = true
        }
      }
    },
    getItemList () {
			this.initSelect()
			this.showSelectedItem = false
			this.selectedItemId = ''
      let params = {
        medicalItemType: this.selectedType,
        pageSize: 99999,
        pageNum: 1,
      }
      this.itemLoading = true
      this.swsApi.swsPost('Data/MedicalItemRecord/list', params).then(res => {
        if (res.data.success) {
          this.itemLoading = false
          this.itemList = res.data.result || []
        }
      })
    },
	}
}
</script>
<style lang="less" scoped>

</style>