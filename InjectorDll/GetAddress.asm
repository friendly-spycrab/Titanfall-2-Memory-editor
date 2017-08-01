CheckIfShouldRedirect PROTO C
.code
GetAddress proc
	push rax
	mov rax,r8
	ret
GetAddress endp


ResetRax proc
	pop rax
	ret
ResetRax endp

EnterRedirect proc
	lea r8,[rsp+30]
	call PushAll ;push all basic registers to the stack. this is so we only overwrite the address
	call CheckIfShouldRedirect
	call PopAll ; pop all basic registers to the stack
	ret
EnterRedirect endp


SetNewAddress proc
	mov r8,rsp
SetNewAddress endp

PushAll proc
	push rax
	push rbx
	push rcx
	push rdx
	push rsi
	push rdi
	push rbp
	push rsp
PushAll endp

PopAll proc
	Pop rax
	Pop rbx
	Pop rcx
	Pop rdx
	Pop rsi
	Pop rdi
	Pop rbp
	Pop rsp
PopAll endp


end

