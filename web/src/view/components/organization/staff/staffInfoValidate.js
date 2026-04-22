export default {
  name: [
    {
      required: true,
      type: 'string',
      message: '请输入姓名',
      trigger: 'blur'
    }
  ],
  idcard: [
    {
      required: true,
      message: '请输入身份证号码',
      trigger: 'blur'
    },
    {
      pattern: /(^\d{15}$)|(^\d{18}$)|(^\d{17}(\d|X|x)$)/,
      message: '请正确输入身份证号码',
      trigger: 'blur'
    }
  ],
  sex: [
    {
      required: true,
      type: 'string',
      message: '请选择性别',
      trigger: 'change'
    }
  ],
  workingState: [
    {
      required: true,
      type: 'string',
      message: '请选择在职情况',
      trigger: 'change'
    },
    {
      type: 'string',
      message: '请选择在职情况',
      trigger: 'blur'
    }
  ],
  phone: [
    {
      required: true,
      type: 'string',
      message: '请输入联系电话',
      trigger: 'blur'
    }
  ],
  birthday: [
    {
      required: true,
      type: 'date',
      message: '请选择出生年月',
      trigger: 'blur'
    },
    {
      type: 'date',
      message: '请正确选择出生年月',
      trigger: 'change'
    }
  ],
  hjAddress: [
    {
      required: true,
      message: '请输入户口所在地地址',
      trigger: 'blur'
    }
  ],
  contactAddress: [
    {
      required: true,
      message: '请输入现居住地',
      trigger: 'blur'
    }
  ],
  depId: [
    {
      required: true,
      message: '请选择部门',
      trigger: 'change'
    },
    {
      message: '请选择部门',
      trigger: 'blur'
    }
  ],
  positionId: [
    {
      required: true,
      message: '请选择职位',
      trigger: 'change'
    },
    {
      message: '请选择职位',
      trigger: 'blur'
    }
  ],
  hiredate: [
    {
      required: true,
      type: 'date',
      message: '请选择入职日期',
      trigger: 'blur'
    },
    {
      type: 'date',
      message: '请正确选择入职日期',
      trigger: 'change'
    }
  ],
  householdRegister: [
    {
      required: true,
      message: '请输入户口性质',
      trigger: 'blur'
    }
  ],
  eduId: [
    {
      required: true,
      message: '请选择学历信息',
      trigger: 'change'
    },
    {
      message: '请选择学历信息',
      trigger: 'blur'
    }
  ],
  major: [
    {
      required: true,
      message: '请输入专业信息',
      trigger: 'blur'
    }
  ]
}
