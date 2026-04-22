<template>
  <Form ref="loginForm" class="form" :model="form" :rules="rules" @keydown.enter.native="handleSubmit">
    <FormItem prop="userName" class="input user">
      <Input prefix="ios-person" v-model="form.userName" placeholder="请输入用户名" />
    </FormItem>
    <FormItem prop="password" class="input">
      <Input prefix="md-lock" type="password" v-model="form.password" placeholder="请输入密码" />
    </FormItem>
    <FormItem class="bg-shadow">
      <Button size="large" :loading="logining" @click="handleSubmit" type="primary" long><span>登录</span></Button>
    </FormItem>
  </Form>
</template>
<script>
import { mapActions } from 'vuex'
export default {
  name: 'LoginForm',
  props: {
    userNameRules: {
      type: Array,
      default: () => {
        return [
          { required: true, message: '账号不能为空', trigger: 'blur' }
        ]
      }
    },
    passwordRules: {
      type: Array,
      default: () => {
        return [
          { required: true, message: '密码不能为空', trigger: 'blur' }
        ]
      }
    }
  },
  data () {
    return {
      form: {
        userName: '',
        password: ''
      },
      logining: false
    }
  },
  computed: {
    rules () {
      return {
        userName: this.userNameRules,
        password: this.passwordRules
      }
    }
  },
  methods: {
    ...mapActions(['handleLogin']),
    handleSubmit () {
      this.$refs.loginForm.validate((valid) => {
        if (valid) {
          this.logining = true
          let _self = this
          this.handleLogin({
            userName: _self.form.userName.trim(),
            pwd: _self.form.password.trim()
          }).then(res => {
            let data = res.data
            if (data.success) {
              // 清除缓存tag列表
              localStorage.tagNaveList = []
              this.$router.push({
                name: 'home'
              })
            } else {
              this.$Message.error(data.error)
            }
            this.logining = false
          }).catch(e => {
            this.logining = false
            console.log(e)
          })
        }
      })
    }
  }
}
</script>
<style lang="less" scoped>
.form {
  /deep/ .ivu-btn {
    position: relative;
    height: 53px;
    font-size: 18px;
    border-radius: 4px;
    z-index: 2;
    span {
      letter-spacing: 10px;
    }
  }

  /deep/ .bg-shadow {
    position: absolute;
    left: 15.2%;
    right: 15.2%;
    bottom: 10%;
    margin-bottom: 0;
    &:after {
      display: block;
      position: absolute;
      visibility: visible;
      content: '';
      width: 90%;
      height: 30px;
      top: 50%;
      left: 50%;
      transform: translate(-50%, -50%);
      box-shadow: 0 12px 20px #50a7fd;
      z-index: 1;
    }
  }
  /deep/ .input {
    margin-bottom: 36px;
    .ivu-form-item-content {
      line-height: 52px;
      .ivu-input {
        padding-left: 60px;
        height: 52px;
        font-size: 14px;
      }
      input:-webkit-autofill {
        box-shadow: 0 0 0 1000px white inset !important;
      }
      .ivu-input-prefix, .ivu-input-suffix {
        width: 45px;
      }
      .ivu-input-prefix i, .ivu-input-suffix i {
        font-size: 24px;
        color: #818999;
        line-height: 52px;
      }
    }
    &.user {
      .ivu-input-prefix i {
        font-size: 30px;
      }
    }
  }
}

@media only screen and (max-width: 1068px) {
  .form {
    /deep/ .bg-shadow {
      left: 6.4%;
      right: 6.4%;
    }
  }
}
</style>
