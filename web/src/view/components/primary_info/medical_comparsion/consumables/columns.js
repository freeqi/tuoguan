export default {
  data() {
    return {
      local_column: [
        {
          title: "项目名",
          width: 180,
          key: "medicalItemName",
          tooltip: true
        },
        {
          title: "规格",
          key: "minDose",
          tooltip: true
        },
        {
          title: "销售价",
          align: "center",
          width: 80,
          render: (h, params) => {
            return <span>{params.row.purchasingPrice || 0}</span>;
          }
        },
        {
          title: "医保中心价",
          align: "center",
          width: 100,
          render: (h, params) => {
            return <span>{params.row.hilist_pric_uplmt_amt || 0}</span>;
          }
        },
        // {
        //   title: '中心编码',
        //   key: 'hiCenterCode',
        //   align: 'center',
        //   minWidth: 80,
        //   tooltip: true
        // },
        {
          title: "国家项目编码",
          key: "nationItemCode",
          width: 200
          // tooltip: true
        },
        {
          title: "厂家",
          key: "manufacturer",
          width: 200,
          tooltip: true
        },
        {
          title: "是否对照",
          minWidth: 80,
          align: "center",
          render: (h, params) => {
            return params.row.nationItemCode !== null ? (
              <span style="color:green">是</span>
            ) : (
              <span style="color:red">否</span>
            );
          }
        }
      ],
      compared_column: [
        {
          title: "项目名",
          key: "medicalItemName",
          tooltip: true
        },
        {
          title: "规格",
          key: "minDose",
          tooltip: true
        },
        {
          title: "销售价",
          align: "center",
          width: 80,
          render: (h, params) => {
            const { purchasingPrice, hilist_pric_uplmt_amt } = params.row;
            if (
              purchasingPrice > hilist_pric_uplmt_amt &&
              hilist_pric_uplmt_amt
            ) {
              return <span style="color: red">{purchasingPrice}</span>;
            } else {
              return <span>{purchasingPrice || 0}</span>;
            }
          }
        },
        {
          title: "医保中心价",
          align: "center",
          width: 80,
          render: (h, params) => {
            return (
              // <span>{params.row.medicalDrugExtension.socialSecurityPrice || 0}</span>
              <span>{params.row.hilist_pric_uplmt_amt || 0}</span>
            );
          }
        },
        {
          title: "等级",
          render: (h, params) => {
            let list = ["/", "甲", "乙", "丙"];
            let index = parseInt(params.row.chrgitm_lv);
            return <span>{isNaN(index) ? list[0] : list[index]}</span>;
          }
        },
        {
          title: "厂家",
          key: "manufacturer",
          width: 200,
          tooltip: true
        },
        // {
        //   title: '中心编码',
        //   align: 'center',
        //   key: 'hiCenterCode',
        //   tooltip: true
        // },
        {
          title: "国家项目编码",
          key: "nationItemCode",
          width: 200
          // tooltip: true
        }
      ],
      center_column: [
        {
          title: "项目名",
          key: "xmmc",
          minWidth: 200,
          tooltip: true
        },
        {
          title: "规格",
          key: "gg",
          minWidth: 100,
          tooltip: true
        },
        {
          title: "规格型号",
          key: "ggxh",
          minWidth: 100,
          tooltip: true
        },
        {
          title: "国家项目代码",
          key: "gjxmdm",
          width: 200
          // tooltip: true
        },
        {
          title: "耗材分类",
          key: "hcfl",
          width: 200,
          tooltip: true
        },
        {
          title: "目录等级",
          key: "ylfydj",
          align: "center",
          minWidth: 100,
          tooltip: true,
          render: (h, params) => {
            let text = this.medical_insurance_type[params.row.ylfydj - 1];
            return <span>{text || "丙"}级</span>;
          }
        },
        {
          title: "自付比例",
          key: "txbl",
          align: "center",
          minWidth: 100,
          tooltip: true
        },
        {
          title: "基准价格",
          key: "ylzdj",
          align: "center",
          minWidth: 120,
          tooltip: true
        },
        {
          title: "医院等级",
          key: "lmttype",
          minWidth: 100,
          tooltip: true,
          render: (h, params) => {
            let str =
              params.row.lmttype == "901"
                ? "一级非公立"
                : params.row.lmttype == "902"
                ? "二级非公立"
                : "";
            return <span>{str}</span>;
          }
        },
        // {
        //   title: '中心编码',
        //   key: 'xmlsh',
        //   tooltip: true,
        //   minWidth: 80
        // },
        // {
        //   title: '国家项目分类',
        //   key: 'gjxmfl',
        //   minWidth: 100,
        //   tooltip: true
        // },
        {
          title: "厂家/限制使用说明",
          key: "bz",
          minWidth: 120,
          tooltip: true
        },
        {
          title: "备注",
          key: "memo",
          minWidth: 220,
          tooltip: true
        },
        {
          title: "变更时间",
          minWidth: 140,
          key: "bgsj",
          render: (h, params) => {
            let date = new Date(params.row.bgsj);
            return <span>{date.toLocaleDateString()}</span>;
          }
        }
      ]
    };
  }
};
