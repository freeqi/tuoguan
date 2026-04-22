/* eslint-disable one-var */
import { accAdd, accMul } from '@/libs/tools.js'
export default {
  data () {
    return {
      // 在6月内,信息技术部完成系统集团端和中心端的采购系数的设置并用颜色标记。
      // 正常颜色为黑色，超过标准的采购系数为红色，提醒审核人员和提需求人员添加备注。要求:
      // 1、天外天供应的物资【有低值易耗、卫生耗材】正常采购系数为2.5以内;
      // 2、药品采购系数为1.5以内;
      // 3、急抢教药品不受采购系数限制;
      // 4、低值易耗品和不收费耗材（非有价卫材）采购系数小于等于2.5。
      // 总结标红的情况：1、(有价卫材 &≠ 天外天)>1.5;2、天外天 > 2.5;3、(是药品且 不是急救药)>1.5
      // 
      ypId: "9807f86a1a8846eb82e85f5a5f92e9e4", //药品
      jjyId: "035558574e784ccea4902e46598986d2", //急救药
      dzyhId: "97277431fc754297853d9d20cb423b50", //低值易耗
      yjwcId: "187b752e52414196b763f91fae53a825", //有价卫材
      ptwcId: "c1fe3b7a2e3a495d958062fa81c9aaf6", //普通卫材
      table_base: [
        {
          title: '序列',
          key: 'no',
          align: 'center',
          width: 40
        },
        {
          title: '机构',
          key: 'centerName',
          tooltip: true,
          minWidth: 50
        },
        {
          title: '物品名称',
          key: 'medicalItemName',
          minWidth: 110
        },
        {
          title: '件比',
          key: 'medicalThan',
          minWidth: 90
        },
        {
          title: '规格',
          key: 'minDose',
          align: 'center',
          minWidth: 70
        },
        {
          title: '厂家',
          key: 'manufacturer',
          tooltip: true,
          minWidth: 120
        },
        {
          title: '供应商',
          key: 'supplierName',
          slot: 'supplierName',
          minWidth: 120
        },
        {
          title: '单位',
          align: 'center',
          key: 'procurementUnitValue',
          minWidth: 50
        },
        {
          title: '上月用量',
          key: 'monthAverage',
          align: 'center',
          minWidth: 65
        },
        {
          title: '库存',
          key: 'currentInventory',
          align: 'center',
          minWidth: 65
        },
        // {
        //   title: '申请数量',
        //   key: 'inQty',
        //   align: 'center',
        //   minWidth: 65
        // },
        {
          title: '采购数量',
          slot: 'approvalQty',
          key: 'approvalQty',
          align: 'center',
          minWidth: 80
        },
        {
          title: '采购系数',
          key: 'coefficient',
          sortable: true,
          align: 'center',
          width: 90,
          // renderHeader: (h, params) => {
          //   return <div style="color:red;display:inline;">采购系数</div>
          // },
          render: (h, params) => {
            let supplierName = params.row.supplierName || ''
            let num = params.row.coefficient || ''
            let wareHouseId = params.row.medicalItemRecordOutPut.wareHouseId || []
            // 总结标红的情况：1、(低值易耗 || 普通卫材)>2.5;2、天外天 > 2.5;3、(是药品且 不是急救药)>1.5
            let isTWT = supplierName.includes('天外天') ? true :false
            let isRed = ((wareHouseId.includes(this.dzyhId)||wareHouseId.includes(this.ptwcId)) && num>2.5) || (isTWT&&num>2.5) || (wareHouseId.includes(this.ypId)&&!wareHouseId.includes(this.jjyId)&&num>1.5)
            if (num&&isRed) {
              return <span style='color:red;'>{num}</span>
            } else {
              // return params.row.id ? <span>{params.row.socialSecurityPrice || '/'}<sup style="color:red"> 未</sup></span> : <span></span>
              return <span>{num}</span>
            }
          }
        },
        {
          title: '采购价(元)',
          key: 'inPrice',
          slot: 'inPrice',
          align: 'center',
          minWidth: 80
        },
        // {
        //   title: '销售价(元)',
        //   key: 'salePrice',
        //   slot: 'salePrice',
        //   align: 'center',
        //   minWidth: 80
        // },
        // {
        //   title: '社保限价(元)',
        //   key: 'socialSecurityPrice',
        //   align: 'center',
        //   width: 70
        // },
        // {
        //   title: '双控价(元)[仅药品]',
        //   key: 'dualPrice',
        //   slot: 'dualPrice',
        //   align: 'center',
        //   width: 85
        // },
        {
          title: '采购总金额(元)',
          key: 'inSumMoney',
          align: 'center',
          minWidth: 65
        },
        // {
        //   title: '销售总金额(元)',
        //   key: 'totalSalePrice',
        //   align: 'center',
        //   minWidth: 65
        // },
        {
          title: '备注',
          slot: 'remark',
          tooltip: true,
          key: 'remark',
          minWidth: 120
        }
      ]
    }
  },
  methods: {
    // cy 合并采购单
    mergePurchaseRecord (selectedIdsArr) {
      let params = ''
      let api = ''
      // cy 20200116 新增合并类型 1：合并生成订单，2：合并采购单
      if (this.mergeType === 1) {
        api = 'CenterDocking/PurchaseDetail/batchList'
        params = {
          applyId: selectedIdsArr
          // supplierId: this.supplierInfo.supplierCheckedId,
          // medicaItemType: this.mergeType // 物品类型
        }
      } else if (this.mergeType === 2) {
        api = 'CenterDocking/Purchase/GroupMergePurchase'
        params = selectedIdsArr
      }
      // console.log('合并！！！！', params)
      this.mergeLoading = true
      this.swsApi.swsPost(api, params)
        .then(res => {
          if (res.data.success) {
            if (this.mergeType === 1) {
              this.handleMergeData(res.data.result)
            } else if (this.mergeType === 2) {
              this.handleFormData(res)
            }
          } else {
            this.$Notice.error({
              title: '请求失败，请稍后再试。',
              desc: res.data.error
            })
          }
          this.mergeLoading = false
        })
        .catch(e => {
          this.mergeLoading = false
          console.log(e)
        })
    },
    // cy 合并时统计数据
    handleMergeData (res) {
      // cy 合计的数据统计
      let itemType = 0, totalQty = 0, actualPrice = 0, salesTotalPrice = 0
      itemType = res.purchaseDetails.length
      res.purchaseDetails.forEach(item => {
        let {medicalItemName, minDose, manufacturer, medicalItemType} = item.medicalItemRecordOutPut
        item.supplierName = item.supplierName || '无'
        // 将这三个数据提出来，为了避免render造成csv导出该对应数据为空的问题
        item.medicalItemName = medicalItemName
        item.minDose = minDose
        item.manufacturer = manufacturer
        // 统计每种物品的采购、销售总金额
        // item.totalInPrice = accMul(item.inQty, item.inPrice)
        item.totalSalePrice = accMul(item.inQty, item.salePrice)

        totalQty = accAdd(totalQty, item.inQty)
        actualPrice = accAdd(actualPrice, item.inSumMoney).toFixed(3)
        salesTotalPrice = accAdd(salesTotalPrice, item.totalSalePrice).toFixed(3)

        item.medicalItemType = medicalItemType
        // return item
      })
      res.totalMergeDetail = {itemType, totalQty, actualPrice: parseFloat(actualPrice), salesTotalPrice: parseFloat(salesTotalPrice)}
      this.mergeData = res
      this.mergeLoading = false
    }
  }
}
