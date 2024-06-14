using System;
using System.Threading;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using UnityEngine.UI;
using JetBrains.Annotations;
using System.Collections;
using UnityEngine.SceneManagement;

public class Authmanager : MonoBehaviour
{
    public string dbURL = "";

    [Header("LogIn Panel")]
    [SerializeField] GameObject logInPanel;
    [SerializeField] TMP_InputField emailInput_logIn;
    [SerializeField] TMP_InputField passwordInput_logIn;

    [SerializeField] Button logInBtn;

    [Header("SignUp Panel")]
    [SerializeField] GameObject SignUpPanel;
    [SerializeField] TMP_InputField userNameInput_signUp;
    [SerializeField] TMP_InputField emailInput_signUp;
    [SerializeField] TMP_InputField passwordInput_signUp;
    [SerializeField] TMP_InputField confirmPasswordInput_signUp;

    [SerializeField] Button signUpBtn;

    [Header("SendMail Panel")]
    [SerializeField] GameObject SendMailPanel;
    [SerializeField] TMP_InputField emailInput_mail;

    [SerializeField] Button sendBtn;
    [SerializeField] Image check;

    [Header("Admin Panel")]
    [SerializeField] GameObject adminMainPanel;
    [SerializeField] TMP_Text userName;

    FirebaseAuth auth;
    FirebaseUser authUser;
    public bool isAuthenticated = false;
    public bool tologin = false;
    public bool tosignup = false;
    public bool tosendmail = false;
    public bool toadminmain = false;

    public void Start()
    {
        emailInput_logIn.text = "valen0705@naver.com";
        passwordInput_logIn.text = "123456789";

        auth = FirebaseAuth.DefaultInstance;

        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new System.Uri(dbURL);
        logInBtn.onClick.AddListener(() => LogIn(emailInput_logIn.text, passwordInput_logIn.text));
        signUpBtn.onClick.AddListener(() => SignUp(userNameInput_signUp.text, emailInput_signUp.text, passwordInput_signUp.text, confirmPasswordInput_signUp.text));

    }

    public void Update()
    {
        if (isAuthenticated && toadminmain)
        {
            logInPanel.SetActive(false);
            SignUpPanel.SetActive(false);
            SendMailPanel.SetActive(false);
            adminMainPanel.SetActive(true);
            userName.text = authUser.DisplayName.ToString();

            tologin = false;
            tosignup = false;
            tosendmail = false;
            toadminmain = false;
        }
        if (tosendmail)
        {
            logInPanel.SetActive(false);
            SignUpPanel.SetActive(false);
            SendMailPanel.SetActive(true);
            tologin = false;
            tosignup = false;

        }
        if(tosignup)
        {
            logInPanel.SetActive(false);
            SignUpPanel.SetActive(true);
            SendMailPanel.SetActive(false);
            tologin = false;
            tosendmail = false;
        }
        if (tologin)
        {
            logInPanel.SetActive(true);
            SignUpPanel.SetActive(false);
            SendMailPanel.SetActive(false);
            tosignup = false;
            tosendmail = false;
        }

    }

    public void LogIn(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                print("�α��� ���� - " + ": \n" + task.Exception);
            }
            else if (task.IsCanceled)
            {
                print("�α��� ��� - " + ": \n" + task.Exception);
            }
            else if (task.IsCompleted)
            {

                AuthResult result = task.Result;
                authUser = result.User;

                if (authUser.IsEmailVerified)
                {
                    print("�α��εǾ����ϴ�. \n ȯ���մϴ�. " + result.User.DisplayName + "��");
                    isAuthenticated = true;
                    toadminmain = true;
                }
                else
                {
                    print("������ ���� ���� �����Դϴ�. ���� ȭ������ �̵��մϴ�...");
                    tosendmail = true;
                }
            }
        });
    }


    public bool SignUp(string name, string email, string password, string confirmedPassword)
    {
        if (name == "" || email == "" || password == "" || confirmedPassword == "")
        {
            print("�Է����� ���� ������ �ֽ��ϴ�. \n ��� ĭ�� ������ �Է����ּ���.");
            return false;
        }
        //DB���� name�� email �ߺ�üũ
        //PW CPW ��ġ���� üũ
        if (password != confirmedPassword)
        {
            print("ȸ������ ����: ��й�ȣ - ��й�ȣ Ȯ���� ����ġ�մϴ�.");

            return false;
        }
        else
        {
            auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    print("ȸ������ ����" + ": " + task.Exception);
                }
                else if (task.IsCanceled)
                {
                    print("ȸ������ ���" + ": " + task.Exception);
                }
                else if (task.IsCompleted)
                {
                    AuthResult result = task.Result;
                    FirebaseUser authUser = result.User;

                    print(result.User.DisplayName + "(" + result.User.UserId + ")���� "
                        + "������ ��û�Ǿ����ϴ�. " +
                        "\n �̸��� ���� ���� ������ �Ϸ�˴ϴ�." +
                        "\n ���� ȭ������ �̵��մϴ�...");
                    tosendmail = true;
                }
            });
            return true;
        }
    }

    UserProfile userProfile;
    public void SendEmailVerification()
    {
        authUser.SendEmailVerificationAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                print("�̸��� ���� ����");
            }

            else if (task.IsCanceled)
            {
                print("�̸��� ���� ���");
            }
            else if (task.IsCompleted)
            {
                print($"���� �޽����� {authUser.Email}�� �����߽��ϴ�.");
            }
        });
    }
}
