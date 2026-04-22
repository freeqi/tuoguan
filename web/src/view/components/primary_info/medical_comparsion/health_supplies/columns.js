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
              <span>{params.row.medicalDrugExtension.retailPrice || 0}</span>
            )
          }
        },
        {
          title: '医保中心价',
          align: 'center',
          render: (h, params) => {
            return (
              <span>{params.row.medicalDrugExtension.socialSecurityPrice || 0}</span>
            )
          }
        },
        {
          title: '生产厂家',
          key: 'manufacturer',
          tooltip: true
        },
        {
          title: '中心编码',
          key: 'hiCenterCode',
          tooltip: true
        }
      ],
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
            const {purchasingPrice, socialSecurityPrice} = params.row.medicalDrugExtension
            if (purchasingPrice > socialSecurityPrice) {
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
              <span>{params.row.medicalDrugExtension.socialSecurityPrice || 0}</span>
            )
          }
        },
        {
          title: '生产厂家',
          key: 'manufacturer',
          tooltip: true
        },
        {
          title: '中心编码',
          key: 'hiCenterCode',
          tooltip: true
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
          title: '剂型',
          key: 'jx',
          tooltip: true,
          minWidth: 100
        },
        {
          title: '包装',
          render: (h, params) => {
            return <span>{params.row.bzsl}{params.row.bzdw}</span>
          },
          tooltip: true,
          minWidth: 120
        },
        {
          title: '含量',
          render: (h, params) => {
            return <span>{params.row.hl}{params.row.hldw}</span>
          },
          tooltip: true,
          minWidth: 120
        },
        {
          title: '容量',
          render: (h, params) => {
            return <span>{params.row.rl}{params.row.rldw}</span>
          },
          tooltip: true,
          minWidth: 120
        },
        {
          title: '生产厂家',
          key: 'ycmc',
          tooltip: true,
          minWidth: 120
        },
        {
          title: '中心编码',
          key: 'yplsh',
          tooltip: true,
          minWidth: 120
        },
        {
          title: '费用分类',
          key: 'lbdm',
          tooltip: true,
          minWidth: 100
        },
        {
          title: '目录等级',
          key: 'ylfydj',
          align: 'center',
          minWidth: 100,
          tooltip: true,
          render: (h, params) => {
            let text = this.medical_insurance_type[params.row.ylfydj - 1]
            return (
              <span>{text}级</span>
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
          key: 'bz',
          minWidth: 120,
          tooltip: true
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
