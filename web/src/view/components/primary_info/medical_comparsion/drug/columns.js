export default {
  data () {
    return {
      local_column: [
        {
          title: '项目名',
          key: 'medicalItemName',
          tooltip: true
        },
        {
          title: '商品名',
          key: 'goodsName',
          tooltip: true
        },
        {
          title: '剂型',
          key: 'formValue',
          width: 100,
          tooltip: true
        },
        {
          title: '包装',
          key: 'packaging',
          tooltip: true
        },
        {
          title: '规格',
          key: 'minDose',
          tooltip: true
        },
        {
          title: '最小剂量',
          key: 'doseMin',
          tooltip: true
        },
        {
          title: '剂量单位',
          key: 'doseUnitValue',
          tooltip: true
        },
        {
          title: '销售价',
          render: (h, params) => {
            return (
              <span>{params.row.purchasingPrice || 0}</span>
            )
          }
        },
        {
          title: '医保中心价',
          align: 'center',
          render: (h, params) => {
            return (
              <span>{params.row.hilist_pric_uplmt_amt || 0}</span>
            )
          }
        },
        {
          title: '生产厂家',
          key: 'manufacturer',
          tooltip: true
        },
        // {
        //   title: '中心编码',
        //   key: 'hiCenterCode',
        //   tooltip: true
        // },
        {
          title: '国家项目编码',
          key: 'nationItemCode',
          width: 200,
          // tooltip: true
        },
        {
          title: '是否对照',
          align: 'center',
          render: (h, params) => {
            return params.row.nationItemCode !== null ? <span style="color:green">是</span> : <span style="color:red">否</span>
          }
        }
      ],
      // 已对照
      compared_column: [
        {
          title: '项目名',
          key: 'medicalItemName',
          tooltip: true
        },
        {
          title: '商品名',
          key: 'goodsName',
          tooltip: true
        },
        {
          title: '剂型',
          key: 'formValue',
          width: 100,
          tooltip: true
        },
        {
          title: '包装',
          key: 'packaging',
          tooltip: true
        },
        {
          title: '规格',
          key: 'minDose',
          tooltip: true
        },
        {
          title: '最小剂量',
          key: 'doseMin',
          tooltip: true
        },
        {
          title: '剂量单位',
          key: 'doseUnitValue',
          tooltip: true
        },
        {
          title: '销售价',
          align: 'center',
          render: (h, params) => {
            const {purchasingPrice, hilist_pric_uplmt_amt} = params.row
            if (purchasingPrice > hilist_pric_uplmt_amt && hilist_pric_uplmt_amt) {
              return (
                <span style="color: red">{purchasingPrice}</span>
              )
            } else {
              return (
                <span>{purchasingPrice || 0}</span>
              )
            }
          }
        },
        {
          title: '医保中心价',
          render: (h, params) => {
            return (
              <span>{params.row.hilist_pric_uplmt_amt || 0}</span>
            )
          }
        },
        {
          title: '等级',
          render: (h, params) => {
            let list = ['/','甲','乙','丙']
            let index = parseInt(params.row.chrgitm_lv)
            return (
              <span>{isNaN(index)?list[0]:list[index]}</span>
            )
          }
        },
        {
          title: '生产厂家',
          key: 'manufacturer',
          tooltip: true
        },
        // {
        //   title: '中心编码',
        //   key: 'hiCenterCode',
        //   tooltip: true
        // },
        {
          title: '国家项目编码',
          key: 'nationItemCode',
          width: 200,
          // tooltip: true
        }
      ],
      center_column: [
        {
          title: '项目名',
          key: 'tym',
          fixed: 'left',
          width: 170,
          tooltip: true
        },
        {
          title: '商品名',
          key: 'spm',
          tooltip: true,
          align: 'center',
          width: 120
        },
        {
          title: '通用名',
          key: 'yptym',
          tooltip: true,
          align: 'center',
          width: 120
        },
        {
          title: '剂型',
          key: 'jx',
          tooltip: true,
          minWidth: 100
        },
        {
          title: '包装单位',
          key: 'bzdw',
          tooltip: true,
          minWidth: 120
        },
        {
          title: '包装材质',
          key: 'bzcz',
          tooltip: true,
          minWidth: 100
        },
        {
          title: '最小包装数量',
          key: 'bzsl',
          tooltip: true,
          minWidth: 120
        },
        {
          title: '最小制剂单位名',
          key: 'zxzjdw',
          tooltip: true,
          minWidth: 120
        },
        {
          title: '含量',
          key: 'hl',
          tooltip: true,
          minWidth: 120
        },
        {
          title: '生产厂家',
          key: 'ycmc',
          tooltip: true,
          minWidth: 120
        },
        // {
        //   title: '容量',
        //   render: (h, params) => {
        //     return <span>{params.row.rl}{params.row.rldw}</span>
        //   },
        //   tooltip: true,
        //   minWidth: 120
        // },
        // {
        //   title: '中心编码',
        //   key: 'yplsh',
        //   tooltip: true,
        //   minWidth: 120
        // },
        {
          title: '国家药品代码',
          key: 'gjypdm',
          width: 200,
          // tooltip: true
        },
        // {
        //   title: '费用分类',
        //   key: 'lbdm',
        //   tooltip: true,
        //   minWidth: 100
        // },
        {
          title: '目录等级',
          key: 'ylfydj',
          align: 'center',
          minWidth: 100,
          tooltip: true,
          render: (h, params) => {
            let text = this.medical_insurance_type[params.row.ylfydj - 1]
            return (
              <span>{text||'丙'}级</span>
            )
          }
        },
        {
          title: '自付比例',
          key: 'ylzfbl',
          minWidth: 100,
          tooltip: true
        },
        {
          title: '基准价格',
          key: 'ylbzdj',
          minWidth: 120,
          tooltip: true
        },
        {
          title: '限制使用说明',
          key: 'xzsyfw',
          minWidth: 120,
          tooltip: true
        },
        {
          title: '医保目录备注',
          key: 'bz',
          minWidth: 120,
          tooltip: true
        },
        {
          title: '批准文号',
          key: 'pzwh',
          minWidth: 120,
          tooltip: true
        },
        {
          title: '医院等级',
          key: 'lmttype',
          minWidth: 100,
          tooltip: true,
          render: (h, params) => {
            let str = params.row.lmttype == '901' 
              ? '一级非公立' : params.row.lmttype == '902'
              ? '二级非公立' : ''
            return (<span>{str}</span>)
          }
        },
        {
          title: '变更时间',
          minWidth: 100,
          key: 'bgsj',
          render: (h, params) => {
            let date = new Date(params.row.bgsj)
            return (
              <span>{date.toLocaleDateString()}</span>
            )
          }
        }
      ]
    }
  }
}
