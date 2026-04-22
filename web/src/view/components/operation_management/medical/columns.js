export default {
  data () {
    return {
      medicalRecordListData2: [
        { i: 3, recordName: '模拟医疗文书1' },
        { i: 4, recordName: '模拟医疗文书2' }
      ],
      historyRecordList: [
      ],
      // demo data
      historyRecordListData: [
        { i: 3, id: '3e90c89089c3424ea8051209380b3f23', recordName: '模拟医疗文书1 2019-04-01', date: '2019-04-01' },
        { i: 3, id: 'e9f8716d6197449db9cacb242ff7dd12', recordName: '模拟医疗文书1 2018-05-24', date: '2018-05-24' },
        { i: 4, id: '3e90c89089c3424ea8051209380b3f23', recordName: '模拟医疗文书2 2019-07-02', date: '2019-07-02' },
        { i: 4, id: 'e9f8716d6197449db9cacb242ff7dd12', recordName: '模拟医疗文书2 2019-05-24', date: '2019-05-24' }
      ],
      // ----------------
      table_detail_columns: [],
      table_detail_data: [],

      // 收费清单table表头
      table_charge_columns: [
        {
          title: '患者姓名',
          key: 'patientName',
          width: 72
        },
        {
          title: '医保类型',
          key: 'siInsuredType',
          align: 'center'
        },
        {
          title: '结算单号',
          key: 'balanceNo'
        },
        {
          title: '结算类别',
          key: 'balanceType',
          align: 'center',
          width: 75,
          render: (h, params) => {
            if (params.row.balanceType === 2) {
              return (<span>自费</span>)
            } else if (params.row.balanceType === 1) {
              return (<span style="color:red;">医保</span>)
            } else if (params.row.balanceType === 3) {
              return (<span>普通医保</span>)
            }
          }
        },
        {
          title: '结算状态',
          key: 'balanceState',
          align: 'center',
          render: (h, params) => {
            if (params.row.balanceState == '0') {
              return <span>已退费</span>
            } else if (params.row.balanceState == '1') {
              return (<span class="status-text-success">正常结算</span>)
            }
          }
        },
        // {
        //   title: '收费人',
        //   key: 'chargePerson'
        // },
        {
          title: '结算时间',
          key: 'balanceDate',
          render: (h, params) => {
            return (
              // <span>{new Date(params.row.balanceDate).toLocaleDateString().replace(/\//g, '-')}</span>
              <span>{this.formateDateToString(new Date(params.row.balanceDate), 'yyyy-MM-dd hh:mm')}</span>
            )
          }
        },
        {
          title: '总金额(元)',
          key: 'sumPrice',
          width: 85
        },
        {
          title: '操作',
          key: 'action',
          align: 'center',
          width: 80,
          render: (h, params) => {
            return (
              <div>
                <tooltip content='查看详情' placement='top'>
                  <icon type='md-list' size='22' color='#4f95e8' onClick={() => { this.showDetail(params.row) }} style={{ cursor: 'pointer' }}></icon>
                </tooltip>
              </div>
            )
          }
        }
      ],
      // 收费清单明细table表头
      table_charge_detail_columns: [
        { title: '项目名称', key: 'itemName' },
        { title: '数量', key: 'qty', width: 60 },
        // { title: '计价单位', key: 'materialTotalUnitName', width: 100 },
        { title: '规格', key: 'specifications' },
        // { title: '剂量', key: 'singleDose' },
        // { title: '频次', key: 'frequencyName' },
        { title: '物品类别', key: 'categoryName' },
        { title: '单价(元)', key: 'unitPrice', width: 68 },
        { title: '总价(元)', key: 'totalPrice', width: 68 },
        { title: '结算人', key: 'balancePerson' },
        {
          title: '结算日期',
          key: 'balanceDate',
          render: (h, params) => {
            return <span>{this.formateDateToString(new Date(params.row.balanceDate), 'yyyy-MM-dd hh:mm:ss')}</span>
          }
        }, {
          title: '处方开单日期',
          key: 'prescriptionDetailFounderDate',
          width: 100,
          render: (h, params) => {
            return <span>{this.formateDateToString(new Date(params.row.prescriptionDetailFounderDate), 'yyyy-MM-dd hh:mm:ss')}</span>
          }
        },
        {
          title: '结算状态',
          key: 'balanceState',
          align: 'center',
          render: (h, params) => {
            if (params.row.balanceState == '0') {
              return <span>已退费</span>
            } else if (params.row.balanceState == '1') {
              return (<span class="status-text-success">正常结算</span>)
            }
          }
        }
        // { title: '备注', key: 'remark', width: 100 },
        // {
        //   title: '已退数量',
        //   key: 'refundTotalQty',
        //   width: 75,
        //   render: (h, params) => {
        //     if (params.row.refundTotalQty == null) {
        //       return h('div', 0)
        //     } else {
        //       return h('div', params.row.refundTotalQty)
        //     }
        //   }
        // }
      ],
      // 处方单table表头
      table_prescription_columns: [
        {
          title: '患者姓名',
          key: 'patientName',
          width: 80
        },
        {
          title: '创建时间',
          key: 'founderDate',
          align: 'center',
          render: (h, params) => {
            return (
              // <span>{new Date(params.row.founderDate).toLocaleDateString().replace(/\//g, '-')}</span>
              <span>{this.formateDateToString(new Date(params.row.founderDate), 'yyyy-MM-dd hh:mm')}</span>
            )
          }
        },
        {
          title: '处方单号',
          key: 'prescriptionNo',
          align: 'center'
        },
        {
          title: '收费类别',
          key: 'chargeType',
          align: 'center',
          width: 80,
          render: (h, params) => {
            if (params.row.chargeType === 1) {
              return (<span>自费</span>)
            } else if (params.row.chargeType === 2) {
              return (<span style="color:red;">特病医保</span>)
            } else if (params.row.chargeType === 3) {
              return (<span>普通医保</span>)
            }
          }
        },
        {
          title: '收费状态',
          key: 'chargeStatus',
          align: 'center',
          width: 80,
          render: (h, params) => {
            if (params.row.chargeStatus === '1') {
              return <span>未收费</span>
            } else if (params.row.chargeStatus === '2') {
              return (<span class="status-text-success">已收费</span>)
            } else if (params.row.chargeStatus === '3') {
              return <span>部分退费</span>
            } else {
              return <span>全部退费</span>
            }
          }
        },
        {
          title: '收费人',
          key: 'chargePerson',
          width: 75
        },
        {
          title: '收费时间',
          key: 'chargeDate',
          align: 'center',
          render: (h, params) => {
            return (
              // <span>{new Date(params.row.chargeDate).toLocaleDateString().replace(/\//g, '-')}</span>
              <span>{this.formateDateToString(new Date(params.row.chargeDate), 'yyyy-MM-dd hh:mm')}</span>
            )
          }
        },
        // {
        //   title: '收费总价(元)',
        //   key: 'totalPrice'
        // },
        {
          title: '操作',
          key: 'action',
          align: 'center',
          width: 80,
          render: (h, params) => {
            return (
              <div>
                <tooltip content='查看详情' placement='top'>
                  <icon type='md-list' size='22' color='#4f95e8' onClick={() => { this.showDetail(params.row) }} style={{ cursor: 'pointer' }}></icon>
                </tooltip>
              </div>
            )
          }
        }
      ],
      // 处方单明细table表头
      table_prescription_detail_columns: [
        { title: '名称', key: 'materialName', width: 180, fixed: 'left' },
        { title: '计价数量', key: 'materialTotal', width: 100 },
        { title: '计价单位', key: 'materialTotalUnitName', width: 100 },
        { title: '规格', key: 'specifications', width: 150 },
        { title: '销售单价(元)', key: 'unitPrice', width: 125 },
        { title: '应收总价(元)', key: 'receivablePrice', width: 125 },
        {
          title: '开单日期',
          key: 'prescriptionDate',
          width: 100,
          render: (h, params) => {
            // return <span>{new Date(params.row.prescriptionDate).toLocaleDateString().replace(/\//g, '-')}</span>
            return <span>{this.formateDateToString(new Date(params.row.prescriptionDate), 'yyyy-MM-dd hh:mm')}</span>
          }
        },
        { title: '剂量', key: 'singleDose', width: 100 },
        { title: '频次', key: 'frequencyName', width: 100 },
        { title: '用法', key: 'usageName', width: 100 },
        {
          title: '已退数量',
          key: 'refundTotalQty',
          width: 75,
          render: (h, params) => {
            if (params.row.refundTotalQty == null) {
              return h('div', 0)
            } else {
              return h('div', params.row.refundTotalQty)
            }
          }
        },
        { title: '退费金额', key: 'refundPrice', width: 100 },
        { title: '备注', key: 'remark', minWidth: 100, tooltip: true }
      ]
    }
  },
  methods: {
    formateDateToString (da, fmt) {
      let o = {
        'M+': da.getMonth() + 1, // 月份
        'd+': da.getDate(), // 日
        'h+': da.getHours(), // 小时
        'm+': da.getMinutes(), // 分
        's+': da.getSeconds(), // 秒
        'q+': Math.floor((da.getMonth() + 3) / 3), // 季度
        'S': da.getMilliseconds() // 毫秒
      }
      if (/(y+)/.test(fmt)) {
        fmt = fmt.replace(RegExp.$1, (da.getFullYear() + '').substring(4 - RegExp.$1.length))
      }
      for (let k in o) {
        if (new RegExp('(' + k + ')').test(fmt)) {
          fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (('00' + o[k]).substring(('' + o[k]).length)))
        }
      }
      return fmt
    }
  }
}
