# KAHS_IP_SETTER
한국애니메이션고등학교 작화실 아이피 자동설정 프로그램 입니다. <br/>
이 프로그램은 관리자 권한을 얻은 조건 하에 작동합니다.

## 원리
학년, 반, 번호를 입력하게 되면 다음과 같은 수식에 따라 IP를 자동으로 설정합니다.

``` 
10.44.(63 - 학년 / 3).(9 + 번호 + 108 * (학년 / 2) - 38 * (학년 / 3) + (반 - 1) * 28)
```
