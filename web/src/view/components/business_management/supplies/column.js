/* eslint-disable */
export default {
	data() {
		return {
			settleBtnLoading: [],
			columns1: [		//进销存汇总表
				{ title: '序号', key: 'serialNumber', width: 55,
					render: (h,params) =>{
						if (params.row.serialNumber === '合计') {
							return <span>合计</span>
						} else {
							let no = params.index + 1 + this.pageSize * (this.pageIndex - 1)
							return <span>{no}</span>
						}
						
					} 
				},
				{ title: '机构', key: 'dialysisName', width: 180, },
				{ title: '名称', key: 'medicalItemName', width: 180, },
				{ title: '国家码', key: 'nationItemCode', width: 180, },
				{ title: '类别',key:'approvalNum',width:120,},
				{ title: '材质', key: 'itemTypeName', width: 80 },
				{ title: '规格', key: 'specifications', width: 180 },
				{ title: '厂家', key: 'manufacturer', width: 240 },
				{ title: '计算单位', key: 'unitName', width: 80 },
				{ title: '成本单价', key: 'inPrice', width: 85 },
				{
					title: '1_初期', align: 'center',
					children: [
						{ title: '数量', key: 'beginCount', align: 'center', width: 70 },
						{ title: '销售总价', key: 'beginSalePrice', align: 'center', width: 80 },
						{ title: '成本总价', key: 'beginCostPrice', align: 'center', width: 80 },
						{ title: '差价', key: 'beginDifference', align: 'center', width: 80 },
					]
				},
				{
					title: '2_入库', align: 'center',
					children: [
						{ title: '数量', key: 'putInStorageCount', align: 'center', width: 70 },
						{ title: '销售总价', key: 'putInStorageSalePrice', align: 'center', width: 80 },
						{ title: '成本总价', key: 'putInStorageCostPrice', align: 'center', width: 80 },
						{ title: '差价', key: 'putInStorageDifference', align: 'center', width: 80 },
					]
				}, 
				{
					title: '2.1_冲销入库', align: 'center',
					children: [
						{ title: '数量', key: 'cxPutInStorageCount', align: 'center', width: 70 },
						{ title: '销售总价', key: 'cxPutInStorageSalePrice', align: 'center', width: 80 },
						{ title: '成本总价', key: 'cxPutInStorageCostPrice', align: 'center', width: 80 },
						{ title: '差价', key: 'cxPutInStorageDifference', align: 'center', width: 80 },
					]
				}, 
				{
					title: '3_其他出库',
					align: 'center',
					children: [
						{ title: '数量', key: 'outboundCount', align: 'center', width: 70 },
						{ title: '销售总价', key: 'outboundSalePrice', align: 'center', width: 80 },
						{ title: '成本总价', key: 'outboundCostPrice', align: 'center', width: 80 },
						{ title: '差价', key: 'outboundDifference', align: 'center', width: 80 }
					]
				},
				{
					title: '4_领用出库',
					key: '',
					align: 'center',
					children: [
						{ title: '数量', key: 'receiveCount', align: 'center', width: 70 },
						{
							title: '销售总价',
							key: 'receiveSalePrice',
							align: 'center',
							width: 80
						},
						{
							title: '成本总价',
							key: 'receiveCostPrice',
							align: 'center',
							width: 80
						},
						{
							title: '差价',
							key: 'receiveDifference',
							align: 'center',
							width: 80
						}
					]
				},
				{
					title: '4.1_冲销领用出库',
					key: '',
					align: 'center',
					children: [
						{ title: '数量', key: 'cxReceiveCount', align: 'center', width: 70 },
						{
							title: '销售总价',
							key: 'cxReceiveSalePrice',
							align: 'center',
							width: 80
						},
						{
							title: '成本总价',
							key: 'cxReceiveCostPrice',
							align: 'center',
							width: 80
						},
						{
							title: '差价',
							key: 'cxReceiveDifference',
							align: 'center',
							width: 80
						}
					]
				},
				{
					title: '5_报废出库',
					key: '',
					align: 'center',
					children: [
						{ title: '数量', key: 'scrapCount', align: 'center', width: 70 },
						{
							title: '销售总价',
							key: 'scrapSalePrice',
							align: 'center',
							width: 80
						},
						{
							title: '成本总价',
							key: 'scrapCostPrice',
							align: 'center',
							width: 80
						},
						{
							title: '差价',
							key: 'scrapDifference',
							align: 'center',
							width: 80
						}
					]
				},
				{
					title: '6_退货出库',
					key: '',
					align: 'center',
					children: [
						{ title: '数量', key: 'returnCount', align: 'center', width: 70 },
						{
							title: '销售总价',
							key: ' ',
							align: 'center',
							width: 80
						},
						{
							title: '成本总价',
							key: 'returnCostPrice',
							align: 'center',
							width: 80
						},
						{
							title: '差价',
							key: 'returnDifference',
							align: 'center',
							width: 80
						}
					]
				},
				{
					title: '7_销售出库', align: 'center',
					children: [
						{ title: '数量', key: 'salesCount', align: 'center', width: 70 },
						{ title: '销售总价', key: 'salesSalePrice', align: 'center', width: 80 },
						{ title: '成本总价', key: 'salesCostPrice', align: 'center', width: 80 },
						{ title: '差价', key: 'salesDifference', align: 'center', width: 80 },
					]
				},
				{
					title: '8_退费退库',
					key: '',
					align: 'center',
					children: [
						{ title: '数量', key: 'refundSalesCount', align: 'center', width: 70 },
						{
							title: '销售总价',
							key: 'refundSalesSalePrice',
							align: 'center',
							width: 80
						},
						{
							title: '成本总价',
							key: 'refundSalesCostPrice',
							align: 'center',
							width: 80
						},
						{
							title: '差价',
							key: 'refundSalesDifference',
							align: 'center',
							width: 80
						}
					]
				},
				{
					title: '9_期末', align: 'center',
					children: [
						{ title: '数量', key: 'endCount', align: 'center', width: 70 },
						{ title: '销售总价', key: 'endSalePrice', align: 'center', width: 80 },
						{ title: '成本总价', key: 'endCostPrice', align: 'center', width: 80 },
						{ title: '差价', key: 'endDifference', align: 'center', width: 80 },
					]
				},
			],
			columns19: [		//进销存汇总表（简单）
				{ title: '序号', key: 'serialNumber', width: 55,
					render: (h,params) =>{
						if (params.row.serialNumber === '合计') {
							return <span>合计</span>
						} else {
							let no = params.index + 1 + this.pageSize * (this.pageIndex - 1)
							return <span>{no}</span>
						}
						
					} 
				},
				{ title: '机构', key: 'dialysisName', width: 180, },
				{ title: '名称', key: 'medicalItemName', width: 180, },
				{ title: '国家码', key: 'nationItemCode', width: 180, },
				{ title: '材质', key: 'itemTypeName', width: 80 },
				{ title: '规格', key: 'specifications', width: 180 },
				{ title: '厂家', key: 'manufacturer', width: 240 },
				{ title: '计算单位', key: 'unitName', width: 80 },
				{ title: '成本单价', key: 'inPrice', width: 85 },
				{
					title: '初期', align: 'center',
					children: [
						{ title: '数量', key: 'beginCount', align: 'center', width: 70 },
						{ title: '销售总价', key: 'beginSalePrice', align: 'center', width: 100 },
						{ title: '成本总价', key: 'beginCostPrice', align: 'center', width: 100 },
						{ title: '差价', key: 'beginDifference', align: 'center', width: 80 },
					]
				},
				{
					title: '入库', align: 'center',
					children: [
						{ title: '数量', key: 'putInStorageCount', align: 'center', width: 70 },
						{ title: '销售总价', key: 'putInStorageSalePrice', align: 'center', width: 100 },
						{ title: '成本总价', key: 'putInStorageCostPrice', align: 'center', width: 100 },
						{ title: '差价', key: 'putInStorageDifference', align: 'center', width: 80 },
					]
				}, 
				{
					title: '出库',
					align: 'center',
					children: [
						{ title: '数量', key: 'outboundCount', align: 'center', width: 70 },
						{ title: '销售总价', key: 'outboundSalePrice', align: 'center', width: 100 },
						{ title: '成本总价', key: 'outboundCostPrice', align: 'center', width: 100 },
						{ title: '差价', key: 'outboundDifference', align: 'center', width: 80 }
					]
				},
				{
					title: '期末', align: 'center',
					children: [
						{ title: '数量', key: 'endCount', align: 'center', width: 70 },
						{ title: '销售总价', key: 'endSalePrice', align: 'center', width: 100 },
						{ title: '成本总价', key: 'endCostPrice', align: 'center', width: 100 },
						{ title: '差价', key: 'endDifference', align: 'center', width: 80 },
					]
				},
			],
			columns2: [		//入出库汇总表
				{ title: '序号', key: 'serialNumber', width: 55,
				  render: (h,params) =>{
					if (params.row.serialNumber === '合计') {
						return <span>合计</span>
					}   else {
							let no = params.index + 1 + this.pageSize * (this.pageIndex - 1)
						return <span>{no}</span>
					}
				  } 
				},
				{ title: '机构名称', key: 'dialysisName' },
				{ title: '物品类别', key: 'medicalItemTypeName' },
				{ title: '业务大类', key: 'parentBusinessType' },
				{ title: '业务分类', key: 'childBusinessType' },
				{ title: '成本总价', key: 'inPriceSum' },
				{ title: '销售总价', key: 'salePriceSum' },
				{ title: '差价', key: 'differencePrice' },
			],
			columns3: [		//近效期查询
				{ title: '序号', key: 'serialNumber', width: 55,
					render: (h,params) =>{
						if (params.row.serialNumber === '合计') {
							return <span>合计</span>
						} else {
							let no = params.index + 1 + this.pageSize * (this.pageIndex - 1)
							return <span>{no}</span>
						}
					} 
				},
				{ title: '机构', key: 'dialysisName', width: 180, },
				{ title: '类别', key: 'itemTypeName', width: 80 },
				{ title: '批次号', key: 'batchNo', width: 140 },
				{ title: '名称', key: 'medicalItemName', width: 180 },
				{ title: '规格', key: 'specifications', width: 140 },
				{ title: '库存', key: 'inQty', width: 80 },
				{ title: '单位', key: 'unitName', width: 80 },
				{
					title: '有效期', key: 'qualityDate', width: 80,
					render: (h, params) => {
						if (params.row.qualityDate) {
							return h('div', params.row.qualityDate.substring(0, 10))
						}
					}
				},
				{ title: '成本单价', key: 'inPrice', width: 80 },
				{ title: '成本总价', key: 'costAmount', width: 80 },
				{ title: '销售单价', key: 'salePrice', width: 80 },
				{ title: '销售总价', key: 'salesAmount', width: 80 },
				{ title: '预警库存', key: 'minInventory', width: 80 },
				{ title: '厂家', key: 'manufacturer', width: 180 },
				{ title: '供应商', key: 'supplierName', width: 180 },
			],
			columns4: [		//外购入库统计
				{ title: '序号', key: 'rowNumber', width: 40,
					render: (h,params) =>{
						if (params.row.rowNumber === '合计') {
							return <span>合计</span>
						} else {
							let no = params.index + 1 + this.pageSize * (this.pageIndex - 1)
							return <span>{no}</span>
						}
					} 
				},
				{ title: '机构', key: 'dialysisName', width: 140, tooltip: true },
				// { title: '供应商', width: 140, tooltip: true, render: (h, params) => {
				// 	let name = this.supplierList.filter(item => {
				// 		return item.id === params.row.supplierId
				// 	})
				// 	return h('span', name.length > 0 ? name[0].name : params.row.id ? '/' : '')
				// } },
				{ title: '供应商', key: 'supplierName', width: 140, tooltip: true },
				{ title: '采购单号', key: 'purchaseNo', width: 120 },
				{ title: '入库单号', key: 'inStorageNo', width: 120 },
				{ title: '单据说明', key: 'inTypeName', width: 100 },
				{ title: '成本总额', key: 'inPriceSum', width: 100 },
				{ title: '销售总额', key: 'salePriceSum', width: 100 },
				{ title: '差价金额', key: 'difference', width: 100 },
				{ title: '填制人', key: 'founderName', width: 80 },
				{
					title: '填制日期', key: 'founderDate', width: 80,
					render: (h, params) => {
						if (params.row.founderDate) {
							return h('div', params.row.founderDate.substring(0, 10))
						}
					}
				},
				{ title: '审核人', key: 'auditPerson', width: 80, },
				{
					title: '审核日期', key: 'auditDate', width: 80,
					render: (h, params) => {
						if (params.row.auditDate) {
							return h('div', params.row.auditDate.substring(0, 10))
						}
					}
				},
				{
					title: '勾稽日期', key: 'checkedDate', width: 80,
					render: (h, params) => {
						if (params.row.checkedDate) {
							return h('div', params.row.checkedDate.substring(0, 10))
						}
					}
				},
				{
					title: '结算日期', key: 'settlementDate', width: 80,
					render: (h, params) => {
						if (params.row.settlementDate) {
							return h('div', params.row.settlementDate.substring(0, 10))
						}
					}
				},
				{ title: '摘要', key: 'remark', minWidth: 200, },
				{ 
					title: '勾稽状态', key: 'isChecked', align: 'center', minWidth: 80, fixed: 'right',
					render: (h, params) => {
						if (params.row.isChecked === 0) {
							return (<i-button type="info" size="small" loading={this.settleBtnLoading.includes(params.row.id)} onClick={()=>{this.handleCheck(params)}}>未勾稽</i-button>)
						  } else if (params.row.isChecked === 1) {
							return (<i-button type="success" size="small">已勾稽</i-button>)
						}
					} 
				},
				{ 
					title: '结算状态', key: 'isSettlement', align: 'center', minWidth: 80, fixed: 'right',
					render: (h, params) => {
						if (params.row.isSettlement === 0) {
							return (<i-button type="primary" size="small" loading={this.settleBtnLoading.includes(params.row.id)} onClick={()=>{this.handleSettle(params)}}>未结算</i-button>)
						  } else if (params.row.isSettlement === 1) {
							return (<i-button type="success" size="small">已结算</i-button>)
						}
					} 
				},
			],
			columns5: [			//外购入库明细
				{ title: '序号', key: 'rowNumber', width: 45,
					render: (h,params) =>{
						if (params.row.rowNumber === '合计') {
							return <span>合计</span>
						} else {
							let no = params.index + 1 + this.pageSize * (this.pageIndex - 1)
							return <span>{no}</span>
						}
						
					} 
				},
				{ title: '物料名称', key: 'medicalItemName', minWidth: 120 },
				{ title: '类别',key:'approvalNum',width:120,},
				// { title: '包装', key: 'procurementPackage', width: 100 },
				// { title: '规格', key: 'specifications', width: 100 },
				{ title: '规格型号', key: 'specifications', width: 120,
					render: (h, params) => {
						// 1.字符串相等，只显示一个（随便显示哪个)
						// 2.不相等，
						return params.row.specifications !== '' && params.row.specifications !== '/'
							? h('div', params.row.specifications)
							: h('div', params.row.procurementPackage)
					}
				},
				{ title: '厂家', key: 'manufacturer', width: 170 },
				{ title: '供应商', key: 'supplierName', width: 170 },
				{ title: '单位', key: 'unitName', width: 60, },
				{ title: '实收数量', key: 'inQty', width: 80, },
				{ title: '采购单价', key: 'inPrice', width: 100, },
				{ title: '采购总价', key: 'costAmount', width: 100, },
				{ title: '销售单价', key: 'salePrice', width: 80, },
				{ title: '销售总价', key: 'salesAmount', width: 100, },
				{ title: '物料类型', key: 'brand', minWidth: 80 },
				{ title: '物料代码', key: 'medicalItemCode', width: 80 },
				{ title: '收料仓库', key: 'medicalItemType', width: 80 },
				{
					title: '日期', key: 'founderDate', width: 80,
					render: (h, params) => {
						if (params.row.founderDate) {
							return h('div', params.row.founderDate.substring(0, 10))
						}
					}
				},
				{ title: '单号', key: 'inStorageNo', width: 140 },
				{ title: '批次号', key: 'batchNo', width: 125 },
				{ title: '有效期', key: 'qualityDate', width: 125,
					render: (h, params) => {
						if (params.row.qualityDate) {
							return h('div', params.row.qualityDate.substring(0, 10))
						}
					}
				},
				{ title: '机构', key: 'dialysisName', width: 170, },
				{
					title: '审核日期', key: 'auditDate', minWidth: 80,
					render: (h, params) => {
						if (params.row.auditDate) {
							return h('div', params.row.auditDate.substring(0, 10))
						}
					}
				},
				{ title: '审核人', key: 'auditPerson', width: 80, },
			],
			columns6: [			//划价出库统计
				//				{title: '患者',key: 'name',},
				{ title: '序号', key: 'serialNumber', width: 65,
					render: (h,params) =>{
						if (params.row.serialNumber === '合计') {
							return <span>合计</span>
						} else {
							let no = params.index + 1 + this.pageSize * (this.pageIndex - 1)
							return <span>{no}</span>
						}
					} 
				},
				{ title: '机构', key: 'dialysisName', minWidth: 180, },
				{ title: '单号', key: 'outboundNo', minWidth: 160 },
				{ title: '成本总价', key: 'costAmount', minWidth: 100 },
				{ title: '销售总价', key: 'salesAmount', minWidth: 100 },
				{ title: '出库类型', key: 'outboundType', minWidth: 90 },
				{ title: '出库时间', key: 'outboundDate', minWidth: 160 },
			],
			columns7: [			//划价出库明细
				{ title: '序号', key: 'serialNumber', width: 55, align: 'center',
					render: (h, params) => {
						
						if (params.row.serialNumber === '合计') {
							return <span>合计</span>
						} else {
							let no = params.index + 1 + this.pageSize * (this.pageIndex - 1)
							return <div style={(params.row.ybscl!==null && params.row.ybscl !== params.row.outInBoundQty) ? 'color:red;' : ''}>{no}</div>
						}
					}
				},
				{ title: '机构', key: 'dialysisName', width: 180, },
				{ title: '患者', key: 'patientName', width: 80 },
				{ title: '编码', key: 'medicalItemCode', width: 120 },
				{ title: '出库类型', key: 'outboundType', width: 90 },
				{
					title: '出库时间', key: 'outboundDate', width: 80,
					render: (h, params) => {
						if (params.row.outboundDate) {
							return h('div', params.row.outboundDate.substring(0, 10))
						}
					}
				},
				{ title: '名称', key: 'medicalItemName', width: 180 },
				{ title: '类别',key:'approvalNum',width:120,},
				{ title: '规格', key: 'specifications', width: 160 },
				{ title: '单位', key: 'unitName', width: 80 },
				{ title: '厂家', key: 'manufacturer', width: 240 },
				{ title: '供应商', key: 'supplierName', width: 200 },
				{ title: '批次号', key: 'batchNo', width: 130 },
				{
					title: '生产日期', key: 'productionDate', width: 80,
					render: (h, params) => {
						if (params.row.productionDate) {
							return h('div', params.row.productionDate.substring(0, 10))
						}
					}
				},
				{
					title: '有效期', key: 'qualityDate', width: 80,
					render: (h, params) => {
						if (params.row.qualityDate) {
							return h('div', params.row.qualityDate.substring(0, 10))
						}
					}
				},
				// { title: '成本单价', key: 'inPrice', width: 80 },
				{ title: '成本单价', key: 'unitPrice', width: 80 },
				{ title: '销售单价', key: 'salePrice', width: 80 },
				{ title: '数量', key: 'outInBoundQty', width: 80 },
				{ title: '成本总价', key: 'costAmount', width: 100 },
				{ title: '销售总价', key: 'salesAmount', width: 100 },
				{
					title: '医保上传量', key: 'ybscl', minWidth: 80, align: 'center',
					render: (h, params) => {
						return <div style={params.row.ybscl !== params.row.outInBoundQty ? 'color:red;font-size:16px;' : ''}>{params.row.ybscl}</div>
					}
				},
			],
			columns8: [			//领用出库统计
				{ title: '序号', key: 'serialNumber', width: 55,
					render: (h,params) =>{
						if (params.row.serialNumber === '合计') {
							return <span>合计</span>
						} else {
							let no = params.index + 1 + this.pageSize * (this.pageIndex - 1)
							return <span>{no}</span>
						}
					} 
				},
				{ title: '机构', key: 'dialysisName', width: 180, },
				{ title: '单号', key: 'outboundNo', width: 150 },
				{ title: '出库类型', key: 'outboundType', minWidth: 90 },
				//				{title: '领用部门',key: 'age'},
				{ title: '成本总价', key: 'costAmount', width: 80 },
				{ title: '销售总价', key: 'salesAmount', width: 80 },
				{ title: '差价金额', key: 'differenceAmount', width: 80 },
				//				{title: '领用人',key: 'name'},
				{ title: '填制人', key: 'name', width: 80 },
				{
					title: '填制日期', key: 'founderDate', width: 80,
					render: (h, params) => {
						if (params.row.founderDate) {
							return h('div', params.row.founderDate.substring(0, 10))
						}
					}
				},
				{ title: '审核人', key: 'auditorName', width: 80 },
				{
					title: '审核日期', key: 'auditDate', width: 80,
					render: (h, params) => {
						if (params.row.auditDate) {
							return h('div', params.row.auditDate.substring(0, 10))
						}
					}
				},
				{ title: '摘要', key: 'remark', minWidth: 200 },
			],
			columns9: [			//领用出库明细
				{ title: '序号', key: 'serialNumber', width: 55,
					render: (h,params) =>{
						if (params.row.serialNumber === '合计') {
							return <span>合计</span>
						} else {
							let no = params.index + 1 + this.pageSize * (this.pageIndex - 1)
							return <span>{no}</span>
						}
					} 
				},
				{ title: '机构', key: 'dialysisName', width: 180, },
				{ title: '单号', key: 'outboundNo', width: 150 },
				{ title: '出库类型', key: 'outboundType', minWidth: 90 },
				{ title: '物品信息', key: 'medicalItemName', width: 230 },
				{ title: '类别',key:'approvalNum',width:120,},
				{ title: '规格', key: 'specifications', width: 180 },
				{ title: '厂家', key: 'manufacturer', width: 180 },
				{ title: '批次号', key: 'batchNo', width: 120 },
				{ title: '供应商', key: 'supplierName', width: 120 },
				{
					title: '失效期', key: 'qualityDate', width: 80,
					render: (h, params) => {
						if (params.row.qualityDate) {
							return h('div', params.row.qualityDate.substring(0, 10))
						}
					}
				},
				{ title: '填写数量', key: 'materialQuantity', width: 100 },
				{ title: '实际数量', key: 'outInBoundQty', width: 100 },
				{ title: '单位', key: 'unitName', width: 80 },
				{ title: '成本单价', key: 'inPrice', width: 80 },
				{ title: '成本总价', key: 'costAmount', width: 100 },
				{ title: '销售单价', key: 'salePrice', width: 80 },
				{ title: '销售总价', key: 'salesAmount', width: 100 },
				{ title: '差价', key: 'differenceAmount', width: 80 },
				{ title: '商品条码', key: 'medicalItemCode', width: 80 },
				{ title: '内部条码', key: 'medicalItemWorkCode', width: 80 },
				{ title: '出库时间', key: 'auditDate', width: 100 },
			],
			columns10: [		//其他出库统计
				{ title: '序号', key: 'serialNumber', width: 55,
					render: (h,params) =>{
						if (params.row.serialNumber === '合计') {
							return <span>合计</span>
						} else {
							let no = params.index + 1 + this.pageSize * (this.pageIndex - 1)
							return <span>{no}</span>
						}
					} 
				},
				{ title: '机构', key: 'dialysisName', width: 180, },
				{ title: '单号', key: 'outboundNo', width: 160 },
				{ title: '出库类型', key: 'outboundType', minWidth: 90 },
				{ title: '成本总价', key: 'costAmount', width: 100 },
				{ title: '销售总价', key: 'salesAmount', width: 100 },
				{ title: '差价金额', key: 'differenceAmount', width: 100 },
				{ title: '填制人', key: 'name', width: 80 },
				{
					title: '填制日期', key: 'founderDate', width: 80,
					render: (h, params) => {
						if (params.row.founderDate) {
							return h('div', params.row.founderDate.substring(0, 10))
						}
					}
				},
				{ title: '审核人', key: 'auditorName', width: 80 },
				{
					title: '审核日期', key: 'auditDate', width: 80,
					render: (h, params) => {
						if (params.row.auditDate) {
							return h('div', params.row.auditDate.substring(0, 10))
						}
					}
				},
				{ title: '摘要', key: 'remark', minWidth: 180 },
			],
			columns11: [		//其他出库明细
				{ title: '序号', key: 'serialNumber', width: 55,
					render: (h,params) =>{
						if (params.row.serialNumber === '合计') {
							return <span>合计</span>
						} else {
							let no = params.index + 1 + this.pageSize * (this.pageIndex - 1)
							return <span>{no}</span>
						}
					} 
				},
				{ title: '机构', key: 'dialysisName', width: 180, },
				{ title: '单号', key: 'outboundNo', width: 160 },
				{ title: '出库类型', key: 'outboundType', minWidth: 90 },
				{ title: '物品信息', key: 'medicalItemName', minWidth: 180 },
				{ title: '规格', key: 'specifications', width: 180 },
				{ title: '厂家', key: 'manufacturer', width: 150 },
				{ title: '批号', key: 'batchNo', width: 100 },
				{ title: '失效期', key: 'qualityDate',
				render: (h, params) => {
					if (params.row.founderDate) {
						return h('div', params.row.founderDate.substring(0, 10))
					}
				} },
				{ title: '数量', key: 'itemQty', width: 80 },
				{ title: '单位', key: 'unitName', width: 80 },
				{ title: '成本单价', key: 'inPrice', width: 80 },
				{ title: '成本总价', key: 'costAmount', width: 80 },
				{ title: '销售单价', key: 'salePrice', width: 80 },
				{ title: '销售总价', key: 'salesAmount', width: 80 },
				{ title: '差价', key: 'differenceAmount', width: 80 },
				{ title: '商品条码', key: 'medicalItemCode', width: 130 },
			],
			columns12: [		//台账
				{ title: '序号', key: 'serialNumber', width: 55,
					render: (h,params) =>{
						if (params.row.serialNumber === '合计') {
							return <span>合计</span>
						} else {
							let no = params.index + 1 + this.pageSize * (this.pageIndex - 1)
							return <span>{no}</span>
						}
					} 
				},
				{ title: '机构', key: 'dialysisName', width: 180, },
				{ title: '名称', key: 'medicalItemName', width: 110 },
				{ title: '规格', key: 'specifications', width: 100 },
				{ title: '单位', key: 'unitName', width: 80 },
				{ title: '出入库类型', key: 'itemTypeName', width: 80 },
				{ title: '单据号', key: 'ruChuKuNo', width: 140 },
				{ title: '摘要', key: 'remark', minWidth: 180 },
				{ title: '批号', key: 'batchNo', width: 100 },
				{
					title: '1_入库', key: 'rk', align: 'center',
					children: [
						{ title: '数量', align: 'center', key: 'putInStorageCount', width: 70 },
						{ title: '销售单价', align: 'center', key: 'putInStorageSale', width: 100 },
						{ title: '销售总价', align: 'center', key: 'putInStorageSalePrice', width: 80 },
						{ title: '成本单价', align: 'center', key: 'putInStorageCost', width: 80 },
						{ title: '成本总价', align: 'center', key: 'putInStorageCostPrice', width: 80 },
						{ title: '差价', align: 'center', key: 'putInStorageDifference', width: 80 },
					]
				},
				{
					title: '2_出库', align: 'center', key: 'ck', align: 'center',
					children: [
						{ title: '数量', align: 'center', key: 'outboundCount', width: 70 },
						{ title: '销售单价', align: 'center', key: 'outboundSale', width: 80 },
						{ title: '销售总价', align: 'center', key: 'outboundSalePrice', width: 80 },
						{ title: '成本单价', align: 'center', key: 'outboundCost', width: 80 },
						{ title: '成本总价', align: 'center', key: 'outboundCostPrice', width: 80 },
						{ title: '差价', align: 'center', key: 'outboundDifference', width: 80 },
					]
				},
				{
					title: '3_结存', align: 'center', key: 'jc', align: 'center',
					children: [
						{ title: '数量', align: 'center', key: 'endCount', width: 70 },
						{ title: '销售总价', align: 'center', key: 'endSalePrice', width: 80 },
						{ title: '成本总价', align: 'center', key: 'endCostPrice', width: 80 },
						{ title: '差价', align: 'center', key: 'endDifference', width: 80 },
					]
				}
			],
			columns13: [
				//退货出库统计
				{ title: '序号', key: 'serialNumber', width: 55,
					render: (h,params) =>{
						if (params.row.serialNumber === '合计') {
							return <span>合计</span>
						} else {
							let no = params.index + 1 + this.pageSize * (this.pageIndex - 1)
							return <span>{no}</span>
						}
					} 
				},
				{ title: '机构', key: 'dialysisName', width: 180, },
				{ title: '供应商', key: 'supplierName', width: 180, },
				{ title: '单号', key: 'outboundNo', width: 160 },
				{ title: '成本总价', key: 'costAmount', width: 80 },
				{ title: '销售总价', key: 'salesAmount', width: 80 },
				{ title: '差价金额', key: 'differenceAmount', width: 80 },
				// { title: '出库类型', key: 'outboundType', minWidth: 90 },
				{ title: '填制人', key: 'name', width: 80 },
				{
					title: '填制日期', key: 'founderDate', width: 80,
					render: (h, params) => {
						if (params.row.founderDate) {
							return h('div', params.row.founderDate.substring(0, 10))
						}
					}
				},
				{ title: '审核人', key: 'auditorName', width: 80 },
				{
					title: '审核日期', key: 'auditDate', width: 80,
					render: (h, params) => {
						if (params.row.auditDate) {
							return h('div', params.row.auditDate.substring(0, 10))
						}
					}
				},
				{ title: '退库人', key: 'returnAuditor', width: 80 },
				{
					title: '退库日期', key: 'returnDate', width: 80,
					render: (h, params) => {
						if (params.row.returnDate) {
							return h('div', params.row.returnDate.substring(0, 10))
						}
					}
				},
				{
					title: '勾稽日期', key: 'checkedDate', width: 80,
					render: (h, params) => {
						if (params.row.checkedDate) {
							return h('div', params.row.checkedDate.substring(0, 10))
						}
					}
				},
				{
					title: '结算日期', key: 'settlementDate', width: 80,
					render: (h, params) => {
						if (params.row.settlementDate) {
							return h('div', params.row.settlementDate.substring(0, 10))
						}
					}
				},
				{ title: '摘要', key: 'remark', minWidth: 180 },
				{ 
					title: '勾稽状态', key: 'isChecked', align: 'center', minWidth: 80, fixed: 'right',
					render: (h, params) => {
						if (params.row.isChecked === 0) {
							return (<i-button type="info" size="small" loading={this.settleBtnLoading.includes(params.row.id)} onClick={()=>{this.handleCheck(params)}}>未勾稽</i-button>)
						  } else if (params.row.isChecked === 1) {
							return (<i-button type="success" size="small">已勾稽</i-button>)
						}
					} 
				},
				{ 
					title: '结算状态', key: 'isSettlement', align: 'center', minWidth: 80, fixed: 'right',
					render: (h, params) => {
						if (params.row.isSettlement === 0) {
							return (<i-button type="info" size="small" loading={this.settleBtnLoading.includes(params.row.id)} onClick={()=>{this.handleSettle(params)}}>未结算</i-button>)
						  } else if (params.row.isSettlement === 1) {
							return (<i-button type="success" size="small">已结算</i-button>)
						}
					} 
				},
			],
			columns14: [		//退货出库明细
				{ title: '序号', key: 'serialNumber', width: 55,
					render: (h,params) =>{
						if (params.row.serialNumber === '合计') {
							return <span>合计</span>
						} else {
							let no = params.index + 1 + this.pageSize * (this.pageIndex - 1)
							return <span>{no}</span>
						}
					} 
				},
				{ title: '机构', key: 'dialysisName', width: 180, },
				{ title: '单号', key: 'outboundNo', width: 160 },
				{ title: '物品信息', key: 'medicalItemName', minWidth: 180 },
				{ title: '规格', key: 'specifications', width: 140 },
				{ title: '厂家', key: 'manufacturer', width: 150 },
				{ title: '供应商', key: 'supplierName', width: 200 },
				{ title: '批号', key: 'batchNo', width: 100 },
				{ title: '失效期', key: 'qualityDate', width: 80,
				render: (h, params) => {
					if (params.row.qualityDate) {
						return h('div', params.row.qualityDate.substring(0, 10))
					}
				} },
				{ title: '数量', key: 'itemQty', width: 80 },
				{ title: '单位', key: 'unitName', width: 80 },
				{ title: '成本单价', key: 'inPrice', width: 80 },
				{ title: '成本总价', key: 'costAmount', width: 80 },
				{ title: '销售单价', key: 'salePrice', width: 80 },
				{ title: '销售总价', key: 'salesAmount', width: 80 },
				// { title: '出库类型', key: 'outboundType', minWidth: 90 },
				{ title: '差价', key: 'differenceAmount', width: 80 },
				{ title: '商品条码', key: 'medicalItemCode', width: 130 },
			],
			columns15: [
				//报废出库统计
				{ title: '序号', key: 'serialNumber', width: 55,
					render: (h,params) =>{
						if (params.row.serialNumber === '合计') {
							return <span>合计</span>
						} else {
							let no = params.index + 1 + this.pageSize * (this.pageIndex - 1)
							return <span>{no}</span>
						}
					} 
				},
				{ title: '机构', key: 'dialysisName', width: 180, },
				// { title: '供应商', key: 'supplierName', width: 180, },
				{ title: '单号', key: 'outboundNo', width: 160 },
				{ title: '成本总价', key: 'costAmount', width: 80 },
				{ title: '销售总价', key: 'salesAmount', width: 80 },
				{ title: '差价金额', key: 'differenceAmount', width: 80 },
				// { title: '出库类型', key: 'outboundType', minWidth: 90 },
				{ title: '填制人', key: 'name', width: 80 },
				{
					title: '填制日期', key: 'founderDate', width: 80,
					render: (h, params) => {
						if (params.row.founderDate) {
							return h('div', params.row.founderDate.substring(0, 10))
						}
					}
				},
				{ title: '审核人', key: 'auditorName', width: 80 },
				{
					title: '审核日期', key: 'auditDate', width: 80,
					render: (h, params) => {
						if (params.row.auditDate) {
							return h('div', params.row.auditDate.substring(0, 10))
						}
					}
				},
				{ title: '摘要', key: 'remark', minWidth: 180 },
			],
			columns16: [		//报废出库明细
				{ title: '序号', key: 'serialNumber', width: 55,
					render: (h,params) =>{
						if (params.row.serialNumber === '合计') {
							return <span>合计</span>
						} else {
							let no = params.index + 1 + this.pageSize * (this.pageIndex - 1)
							return <span>{no}</span>
						}
					} 
				},
				{ title: '机构', key: 'dialysisName', width: 180, },
				{ title: '单号', key: 'outboundNo', width: 160 },
				{ title: '物品信息', key: 'medicalItemName', minWidth: 180 },
				{ title: '规格', key: 'specifications', width: 140 },
				{ title: '厂家', key: 'manufacturer', width: 150 },
				{ title: '批号', key: 'batchNo', width: 100 },
				{ title: '失效期', key: 'qualityDate', width: 80,
				render: (h, params) => {
					if (params.row.qualityDate) {
						return h('div', params.row.qualityDate.substring(0, 10))
					}
				} },
				{ title: '数量', key: 'itemQty', width: 80 },
				{ title: '单位', key: 'unitName', width: 80 },
				{ title: '成本单价', key: 'inPrice', width: 80 },
				{ title: '成本总价', key: 'costAmount', width: 80 },
				{ title: '销售单价', key: 'salePrice', width: 80 },
				{ title: '销售总价', key: 'salesAmount', width: 80 },
				// { title: '出库类型', key: 'outboundType', minWidth: 90 },
				{ title: '差价', key: 'differenceAmount', width: 80 },
				{ title: '商品条码', key: 'medicalItemCode', width: 130 },
			],
			columns17: [		//库存统计
				{ title: '序号', key: 'serialNumber', width: 55,
					render: (h,params) =>{
						if (params.row.serialNumber === '合计') {
							return <span>合计</span>
						} else {
							let no = params.index + 1 + this.pageSize * (this.pageIndex - 1)
							return <span>{no}</span>
						}
						
					} 
				},
				{ title: '机构', key: 'dialysisName', width: 180, },
				{ title: '名称', key: 'medicalItemName', width: 180, },
				{ title: '材质', key: 'itemTypeName', width: 80 },
				{ title: '规格', key: 'specifications', width: 180 },
				{ title: '厂家', key: 'manufacturer', width: 240 },
				{ title: '供应商', key: 'supplierName', width: 180 },
				{ title: '计算单位', key: 'unitName', width: 80 },
				{ title: '成本单价', key: 'inPrice', width: 85 },
				{
					title: '1_初期', align: 'center',
					children: [
						{ title: '数量', key: 'beginCount', align: 'center', width: 70 },
						{ title: '成本总价', key: 'beginCostPrice', align: 'center', width: 80 },
					]
				},
				{
					title: '2_入库', align: 'center',
					children: [
						{ title: '数量', key: 'putInStorageCount', align: 'center', width: 70 },
						{ title: '成本总价', key: 'putInStorageCostPrice', align: 'center', width: 80 },
					]
				}, 
				{
					title: '2.1_冲销入库', align: 'center',
					children: [
						{ title: '数量', key: 'cxPutInStorageCount', align: 'center', width: 70 },
						{ title: '成本总价', key: 'cxPutInStorageCostPrice', align: 'center', width: 80 },
					]
				}, 
				{
					title: '3_其他出库',
					align: 'center',
					children: [
						{ title: '数量', key: 'outboundCount', align: 'center', width: 70 },
						{ title: '成本总价', key: 'outboundCostPrice', align: 'center', width: 80 },
					]
				},
				{
					title: '4_领用出库',
					key: '',
					align: 'center',
					children: [
						{ title: '数量', key: 'receiveCount', align: 'center', width: 70 },
						{
							title: '成本总价',
							key: 'receiveCostPrice',
							align: 'center',
							width: 80
						},
					]
				},
				{
					title: '4.1_冲销领用出库',
					key: '',
					align: 'center',
					children: [
						{ title: '数量', key: 'cxReceiveCount', align: 'center', width: 70 },
						{
							title: '成本总价',
							key: 'cxReceiveCostPrice',
							align: 'center',
							width: 80
						},
					]
				},
				{
					title: '5_报废出库',
					key: '',
					align: 'center',
					children: [
						{ title: '数量', key: 'scrapCount', align: 'center', width: 70 },
						{
							title: '成本总价',
							key: 'scrapCostPrice',
							align: 'center',
							width: 80
						},
					]
				},
				{
					title: '6_退货出库',
					key: '',
					align: 'center',
					children: [
						{ title: '数量', key: 'returnCount', align: 'center', width: 70 },
						{
							title: '成本总价',
							key: 'returnCostPrice',
							align: 'center',
							width: 80
						},
					]
				},
				{
					title: '7_销售出库', align: 'center',
					children: [
						{ title: '数量', key: 'salesCount', align: 'center', width: 70 },
						{ title: '成本总价', key: 'salesCostPrice', align: 'center', width: 80 },
					]
				},
				{
					title: '8_退费退库',
					key: '',
					align: 'center',
					children: [
						{ title: '数量', key: 'refundSalesCount', align: 'center', width: 70 },
						{
							title: '成本总价',
							key: 'refundSalesCostPrice',
							align: 'center',
							width: 80
						},
					]
				},
				{
					title: '9_期末', align: 'center',
					children: [
						{ title: '数量', key: 'endCount', align: 'center', width: 70 },
						{ title: '成本总价', key: 'endCostPrice', align: 'center', width: 80 },
					]
				},
			],
			columns18: [
				//物品实际出入库
				{ title: '序号', key: 'serialNumber', width: 55,
					// render: (h,params) =>{
					// 	if (params.row.serialNumber === '合计') {
					// 		return <span>合计</span>
					// 	} else {
					// 		let no = params.index + 1 + this.pageSize * (this.pageIndex - 1)
					// 		return <span>{no}</span>
					// 	}
						
					// } 
				},
				{ title: '物品', key: 'medicalItemName', minWidth: 140 },
				{ title: '规格', key: 'specifications', minWidth: 160 },
				{ title: '厂家', key: 'manufacturer', minWidth: 160 },
				{ title: '单位', key: 'unitName', width: 100 },
				{ title: '类型', key: 'itemTypeName', width: 100, },
				{ title: '期初', key: 'beginCount', width: 120 },
				{ title: '销售(实际出库)', key: 'salesCount', width: 120 },
				{ title: '实际入库', key: 'putInStorageCount', width: 120, },
				{ title: '期末', key: 'endCount', width: 120 },
			]
		}
	},
	methods: {
		handleSettle (params) {
			this.handleIndex = 2
			this.settleData.id = [params.row.id]
			this.settleData.settlementDate = params.row.settlementDate || ''
			if ([4,13].includes(this.recLi) && params.row.isChecked === 0) {
				this.$Message.warning('该明细还未勾稽，请先进行勾稽操作')
				return false
			}
			this.settleModal = true
		},
		handleCheck (params) {
			this.handleIndex = 1
			this.settleData.id = [params.row.id]
			this.settleData.settlementDate = params.row.checkedDate || ''
			this.settleModal = true
		}
	},
}